use std::net::TcpStream;
use std::io::{BufReader, BufWriter, Write, BufRead};
use std::str::FromStr;
use super::GoEngine;
use super::GameResult;
use core::Coord;

struct NetEngine<'a> {
    tcp: TcpStream,
    reader: BufReader<&'a TcpStream>,
    writer: BufWriter<&'a TcpStream>,
}

impl<'a> NetEngine<'a> {
    fn connect(ip: &str, port: u16) -> Self {
        let tcp = TcpStream::connect((ip, port)).unwrap();
        NetEngine {
            tcp: tcp,
            reader: BufReader::new(&tcp),
            writer: BufWriter::new(&tcp),
        }
    }
}

impl<'a> GoEngine for NetEngine<'a> {
    fn greet(&mut self, name: &str) -> Result<(i32, f32), &str> {
        writeln!(&mut self.writer, "LOGIN {}", name);
        self.writer.flush();

        let mut msg = String::new();
        let mut res = self.reader.read_line(&mut msg);
        if res.is_err() || !msg.contains("HELLO") {
            return Err("Error reading greeting response");
        }

        res = self.reader.read_line(&mut msg);
        if res.is_err() || !msg.contains("INFO") {
            return Err("Error communicating game info");
        }

        let segs: Vec<&str> = msg.split_whitespace().collect();
        let size = i32::from_str(segs[1]).map_err(|e| "Error reading game info");
        let time = f32::from_str(segs[2]).map_err(|e| "Error reading game info");

        size.and_then(|s| time.map(|t| (s, t)))
    }

    fn send_move(&mut self, _move: &Coord) -> Result<Coord, GameResult> {
        Err(GameResult::Win)
    }
}
