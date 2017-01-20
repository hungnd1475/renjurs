pub mod net;

use core::Coord;

pub trait GoEngine {
    fn greet(&mut self, name: &str) -> Result<(i32, f32), &str>;
    fn send_move(&mut self, _move: &Coord) -> Result<Coord, GameResult>;
}

pub enum GameResult {
    Draw,
    Lose,
    Win,
}
