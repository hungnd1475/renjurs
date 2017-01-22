use std::ops::Index;
use std::ops::IndexMut;
use super::Coord;

#[derive(Debug, Copy, Clone)]
pub enum Direction {
    Vert,
    Horz,
    DgDwn,
    DgUp,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Color {
    Empty,
    Black,
    White,
}

pub struct Board {
    pub size: usize,
    pub table: Vec<Color>,
}

impl Index<Coord> for Board {
    type Output = Color;

    fn index<'a, 'b>(&'a self, index: Coord) -> &'a Color {
        &self.table[index.x * self.size + index.y]
    }
}

impl IndexMut<Coord> for Board {    
    fn index_mut<'a>(&'a mut self, index: Coord) -> &'a mut Color {
        &mut self.table[index.x * self.size + index.y]
    }
}  

impl Board {
    pub fn new(size: usize) -> Self {
        Board {
            size: size,
            table: vec![Color::Empty; size * size]
        }
    }

    pub fn translate(&self, c: Coord, dir: Direction, step: usize, rev: bool) -> Option<Coord> {
        fn trans_str(val: usize, step: usize, size: usize, rev: bool) -> Option<usize> {
            let tv = if !rev {
                val.checked_add(step)
            } else {
                val.checked_sub(step)
            };
            tv.and_then(|v| if v < size { Some(v) } else { None })
        }

        match dir {
            Direction::Vert => {
                let tx = trans_str(c.x, step, self.size, rev);
                tx.map(|x| Coord::new(x, c.y))
            }
            Direction::Horz => {
                let ty = trans_str(c.y, step, self.size, rev);
                ty.map(|y| Coord::new(c.x, y))
            }
            Direction::DgDwn => {
                let tx = trans_str(c.x, step, self.size, rev);
                let ty = trans_str(c.y, step, self.size, rev);
                tx.and_then(|x| ty.map(|y| Coord::new(x, y)))
            }
            Direction::DgUp => {
                let tx = trans_str(c.x, step, self.size, !rev);
                let ty = trans_str(c.y, step, self.size, rev);
                tx.and_then(|x| ty.map(|y| Coord::new(x, y)))
            }
        }
    }

    fn inline_dir(&self, c1: Coord, c2: Coord, step: usize, dir: Direction) -> bool {
        let lc1 = self.translate(c1, dir, step, false);
        let rc1 = self.translate(c1, dir, step, true);
        match lc1.and_then(|l| rc1.map(|r| (l, r))) {
            None => false,
            Some((l, r)) => l == c2 || r == c2,
        }
    }

    pub fn inline(&self, c1: Coord, c2: Coord, step: usize) -> bool {
        self.inline_dir(c1, c2, step, Direction::Vert) ||
        self.inline_dir(c1, c2, step, Direction::Horz) ||
        self.inline_dir(c1, c2, step, Direction::DgDwn) ||
        self.inline_dir(c1, c2, step, Direction::DgUp)
    }
}