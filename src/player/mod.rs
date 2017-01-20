use core::Coord;

pub trait GoPlayer {
    fn respond(&self, _move: &Coord) -> Coord;
}
