# Jelly Physics

## Overview

This game is a physics-based 2D game inspired by Angry Birds. The player launches
jelly characters into structures made from destructible blocks.

The main focus of the project was experimenting with **soft-body physics** by
creating a jelly using multiple rigidbodies and colliders connected together
with Spring Joints.

The project also includes destructible environments, impact-based damage,
combos, scoring, limited jelly usage and level progression.

---

## Features

* Jelly characters are constructed using multiple Rigidbody2D components and
  colliders connected together using SpringJoint2D.

* The connected rigidbodies allow the jelly to deform and react to collisions
  while still maintaining its overall shape.

* Destructible blocks have health and can be damaged through physics impacts.

* Damage is based on the impact and collision of the jelly with the blocks.

* Each jelly keeps track of which blocks it has already damaged to prevent
  multiple body parts from dealing damage to the same block.

* Blocks can also take damage from falling and colliding with other objects.

* A combo system rewards the player for destroying multiple blocks in
  succession.

* The scoring system takes block destruction, impact damage, combos and
  remaining jellies into account.

* Players have a limited number of jellies available for each level.

* Remaining jellies can provide bonus points when completing a level.

* Levels are completed when all required destructible blocks have been
  destroyed.

* A level completion system displays the player's results and allows them to
  progress through the level selection system.

* The game includes UI for tracking the current score, combo, remaining
  jellies and level progress.

---

## Soft-Body Physics

The jelly does not use a traditional soft-body physics solver.

Instead, the body is constructed from several rigidbodies that are connected
using SpringJoint2D components.

Each individual body section can respond to physics independently, while the
springs pull the sections back towards each other. This creates a simple
soft-body-like effect using Unity's built-in 2D physics system.

This approach allowed me to experiment with:

* Rigidbody physics
* Spring forces
* Collision detection
* Momentum and impact forces
* Multiple colliders acting as one character
* Physics-based deformation

---

## Tech Used / Dependencies

* Unity
* C#
* Unity 2D Physics
* Rigidbody2D
* Collider2D
* SpringJoint2D
* Unity UI

---

## Project Goals

The main goal of this project was to experiment with physics-based character
deformation while creating a complete and playable game around the mechanic.

The Angry Birds-style gameplay loop provided a way to test the jelly physics
in different situations, including launching, collisions, impacts and
destruction.

The project also allowed me to explore how physics systems can interact with
other gameplay systems such as damage, scoring, combos and level progression.
