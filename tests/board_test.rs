extern crate renju;

use renju::core::board::{Board, Color, Direction};
use renju::core::Coord;

#[test]
fn board_init() {
    let brd = Board::new(12);
    assert_eq!(brd.size, 12);
    assert_eq!(brd.table.len(), 12 * 12);
}

#[test]
fn board_index() {
    let mut brd = Board::new(12);
    let c = Coord::new(2, 3);
    brd[c] = Color::Black;

    assert_eq!(brd[c], Color::Black);
}

#[test]
fn board_trans() {
    let brd = Board::new(12);
    let c = Coord::new(6, 3);
    let step = 2;

    let vert = brd.translate(c, Direction::Vert, step, false);
    let horz = brd.translate(c, Direction::Horz, step, false);
    let dgdwn = brd.translate(c, Direction::DgDwn, step, false);
    let dgup = brd.translate(c, Direction::DgUp, step, false);

    assert_eq!(vert, Some(Coord::new(8, 3)));
    assert_eq!(horz, Some(Coord::new(6, 5)));
    assert_eq!(dgdwn, Some(Coord::new(8, 5)));
    assert_eq!(dgup, Some(Coord::new(4, 5)));
}

#[test]
fn board_trans_oob() {
    let brd = Board::new(12);
    let c = Coord::new(6, 3);
    let step = 20;
    let vert = brd.translate(c, Direction::Vert, step, false);

    assert_eq!(vert, None);
}

#[test]
fn board_inline() {
    let brd = Board::new(12);
    let c1 = Coord::new(2, 5);
    let c2 = Coord::new(3, 5);
    let c3 = Coord::new(2, 1);
    let c4 = Coord::new(3, 3);

    assert!(brd.inline(c1, c2, 1));
    assert!(brd.inline(c1, c3, 4));
    assert!(!brd.inline(c1, c4, 2));
    assert!(!brd.inline(c1, c4, 1));
}