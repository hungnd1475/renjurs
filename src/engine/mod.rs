pub mod net;

use core::Coord;

pub trait Engine {
    fn greet(&self, name: &str) -> Result<(usize, f32), &str>;
    fn send_move(&self, _move: Option<Coord>) -> Result<Coord, Ending>;
}

pub enum Ending {
    Draw,
    Lose,
    Win,
}