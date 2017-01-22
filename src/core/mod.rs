pub mod board;

use std::fmt::{Display, Formatter, Error};

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
pub struct Coord {
    pub x: usize,
    pub y: usize,
}

impl Display for Coord {
    fn fmt(&self, f: &mut Formatter) -> Result<(), Error> {
        write!(f, "({:?}, {:?})", self.x, self.y)
    }
}

impl Coord {
    pub fn new(x: usize, y: usize) -> Self {
        Coord { x: x, y: y }
    }
}