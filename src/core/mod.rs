pub mod board;

use std::fmt::{Display, Formatter, Error};

#[derive(Debug)]
pub struct Coord {
    x: i32,
    y: i32,
}

impl Display for Coord {
    fn fmt(&self, f: &mut Formatter) -> Result<(), Error> {
        write!(f, "({:?}, {:?})", self.x, self.y)
    }
}

impl PartialEq for Coord {
    fn eq(&self, other: &Coord) -> bool {
        self.x == other.x && self.y == other.y
    }
}

impl Eq for Coord {}

pub enum Direction {
    Vertical,
    Horizontal,
    DiagDown,
    DiagUp,
}

impl Coord {
    pub fn is_at(&self, other: &Coord, d: i32) -> bool {
        fn check_dir(src: &Coord, trg: &Coord, d: i32, dir: Direction) -> bool {
            src.advance(&dir, d) == *trg || src.advance(&dir, -d) == *trg
        }

        check_dir(self, other, d, Direction::Vertical) ||
        check_dir(self, other, d, Direction::Horizontal) ||
        check_dir(self, other, d, Direction::DiagDown) ||
        check_dir(self, other, d, Direction::DiagUp)
    }

    pub fn advance(&self, dir: &Direction, step: i32) -> Self {
        match *dir {
            Direction::Vertical => {
                Coord {
                    x: self.x + step,
                    y: self.y,
                }
            }
            Direction::Horizontal => {
                Coord {
                    x: self.x,
                    y: self.y + step,
                }
            }
            Direction::DiagDown => {
                Coord {
                    x: self.x + step,
                    y: self.y + step,
                }
            }
            Direction::DiagUp => {
                Coord {
                    x: self.x - step,
                    y: self.y + step,
                }
            }
        }
    }
}
