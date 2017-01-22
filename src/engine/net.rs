use std::net::TcpStream;
use std::io::{BufReader, BufWriter, Write, BufRead};
use std::str::FromStr;
use super::{Engine, Ending};
use core::Coord;

struct NetEngine {
    tcp: TcpStream,
}

impl NetEngine {
    pub fn connect(ip: &str, port: u16) -> Self {
        NetEngine { tcp: TcpStream::connect((ip, port)).unwrap() }
    }
}

impl Engine for NetEngine {
    fn greet(&self, name: &str) -> Result<(usize, f32), &str> {
        let mut reader = BufReader::new(&self.tcp);
        let mut writer = BufWriter::new(&self.tcp);

        writeln!(&mut writer, "LOGIN {}", name);
        writer.flush();

        let mut msg = String::new();
        let mut res = reader.read_line(&mut msg);
        if res.is_err() || !msg.contains("HELLO") {
            return Err("Error reading greeting response");
        }

        res = reader.read_line(&mut msg);
        if res.is_err() || !msg.contains("INFO") {
            return Err("Error communicating game info");
        }

        let segs: Vec<&str> = msg.split_whitespace().collect();
        let size = usize::from_str(segs[1]).map_err(|e| "Error reading board size");
        let time = f32::from_str(segs[2]).map_err(|e| "Error reading timeout");

        size.and_then(|s| time.map(|t| (s, t)))
    }

    fn send_move(&self, _move: Option<Coord>) -> Result<Coord, Ending> {
        let mut reader = BufReader::new(&self.tcp);
        let mut writer = BufWriter::new(&self.tcp);

        let mut msg = String::new();
        match _move {
            None => {}
            Some(m) => {
                writeln!(&mut writer, "MOVE {} {}", m.x + 1, m.y + 1);
                writer.flush();

                let _ = reader.read_line(&mut msg);
                if msg.contains("ERROR") {
                    return Err(Ending::Lose);
                }
            }
        }

        let _ = reader.read_line(&mut msg);
        if msg.contains("MOVE") {
            let segs: Vec<&str> = msg.split_whitespace().collect();
            let rx = usize::from_str(segs[1]).map(|x| x - 1).map_err(|e| Ending::Lose);
            let ry = usize::from_str(segs[2]).map(|y| y - 1).map_err(|e| Ending::Lose);
            return rx.and_then(|x| ry.map(|y| Coord::new(x, y)));
        } else if msg.contains("WIN") {
            return Err(Ending::Win);
        } else if msg.contains("LOSE") {
            return Err(Ending::Lose);
        } else {
            return Err(Ending::Draw);
        }
    }
}
