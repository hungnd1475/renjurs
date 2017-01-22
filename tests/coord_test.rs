extern crate renju;

use renju::core::Coord;

#[test]
fn coord_init() {
    let c = Coord::new(1, 2);
    assert_eq!(c.x, 1);
    assert_eq!(c.y, 2);
}

#[test]
fn coord_eq() {
    let c1 = Coord::new(1, 2);
    let c2 = Coord::new(1, 2);
    assert_eq!(c1, c2);
}

#[test]
fn coord_neq() {
    let c1 = Coord::new(1, 2);
    let c2 = Coord::new(2, 1);
    assert!(c1 != c2);
}