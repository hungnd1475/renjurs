pub mod core;
pub mod engine;
pub mod player;

pub fn play(name: &str, eng: &engine::GoEngine) {
    let (size, time) = eng.greet(name).unwrap();
}