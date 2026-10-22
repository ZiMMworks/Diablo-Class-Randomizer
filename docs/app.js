// Diablo Class Randomizer - web version

"use strict";

// Game data (updated October 2026)

const GAMES = [
  // Bard and Barbarian are Hellfire's hidden classes (playable in DevilutionX or with command.txt).
  game("Diablo", "diablo1", "Bard Barbarian Monk Rogue Sorcerer Warrior"),
  // Warlock added with the Reign of the Warlock DLC (2026).
  game("Diablo II: Resurrected", "diablo2", "Amazon Assassin Barbarian Druid Necromancer Paladin Sorceress Warlock"),
  game("Diablo III", "diablo3", "Barbarian Crusader Demon_Hunter Monk Necromancer Witch_Doctor Wizard"),
  // Paladin and Warlock added with the Lord of Hatred expansion (2026).
  game("Diablo IV", "diablo4", "Barbarian Druid Necromancer Paladin Rogue Sorcerer Spiritborn Warlock"),
  // Tempest (2024), Druid (2025) and Warlock (2026) added after launch.
  game("Diablo Immortal", "diabloimmortal",
    "Barbarian Blood_Knight Crusader Demon_Hunter Druid Monk Necromancer Tempest Warlock Wizard"),
];

function game(name, key, classList) {
  return {
    name,
    logo: `assets/images/${key}.jpg`,
    background: `assets/images/result_${key}.png`,
    classes: classList.split(" ").map((c) => c.replace("_", " ")),
  };
}

const findGame = (name) => GAMES.find((g) => g.name === name);

// Randomizer: never gives the exact same class twice in a row for the same game.

const pickAny = (items) => items[Math.floor(Math.random() * items.length)];

let lastGame = null;
let lastClass = null;

function rollClass(g) {
  let picked;
  do {
    picked = pickAny(g.classes);
  } while (g.classes.length > 1 && g === lastGame && picked === lastClass);

  lastGame = g;
  lastClass = picked;
  return picked;
}

function pickDifferent(items, current) {
  if (items.length < 2) return items[0];
  let next;
  do {
    next = pickAny(items);
  } while (next === current);
  return next;
}

// Sound (Web Audio so sounds can play on a timer, even on phones).
// A new sound stops the one before it.

const Sound = {
  context: null,
  buffers: {},
  current: null,
  names: ["freshmeat", "legendary"],

  // Phones only allow audio after the user taps something, so this runs on the first tap.
  unlock() {
    if (this.context) {
      if (this.context.state === "suspended") this.context.resume();
      return;
    }

    const AudioCtx = window.AudioContext || window.webkitAudioContext;
    if (!AudioCtx) return;

    this.context = new AudioCtx();
    this.names.forEach((name) => {
      fetch(`assets/sounds/${name}.mp3`)
        .then((response) => response.arrayBuffer())
        .then((data) => new Promise((resolve, reject) => this.context.decodeAudioData(data, resolve, reject)))
        .then((buffer) => { this.buffers[name] = buffer; })
        .catch(() => {});
    });
  },

  play(name) {
    const buffer = this.buffers[name];
    if (!this.context || !buffer) return;

    if (this.current) {
      try { this.current.stop(); } catch (e) { /* already finished */ }
    }

    const source = this.context.createBufferSource();
    source.buffer = buffer;
    source.connect(this.context.destination);
    source.start();
    this.current = source;
  },
};

// Screens

const $ = (id) => document.getElementById(id);
const stage = $("stage");
let currentScreen = null;

function showScreen(id, background) {
  document.querySelectorAll(".screen").forEach((screen) => { screen.hidden = screen.id !== id; });
  currentScreen = id;

  const path = background !== undefined ? background : $(id).dataset.bg;
  stage.style.backgroundImage = path ? `url("${path}")` : "none";
  $("backdrop").style.backgroundImage = path ? `url("${path}")` : "none";

  if (id === "screen-title") Embers.start();
  else Embers.stop();
}

// Sets text on an outlined text element (the gradient copy reads data-text).
function setText(element, text) {
  element.textContent = text;
  element.dataset.text = text;
}

// Shrinks a text element's font until it fits its width (long class names).
function fitText(element, baseSize) {
  element.style.fontSize = baseSize + "px";
  const maxWidth = element.clientWidth - 8;
  const range = document.createRange();
  range.selectNodeContents(element);
  const width = range.getBoundingClientRect().width / currentScale;
  if (width > maxWidth && width > 0) {
    element.style.fontSize = (baseSize * maxWidth / width).toFixed(1) + "px";
  }
}

// Title screen embers

const Embers = {
  canvas: $("embers"),
  list: [],
  frame: 0,
  running: false,

  create(startY) {
    return {
      x: Math.random() * 384,
      y: startY,
      speed: 0.4 + Math.random() * 1.1,
      size: 1.5 + Math.random() * 2.5,
      drift: Math.random() * Math.PI * 2,
    };
  },

  start() {
    if (this.running) return;
    if (!this.list.length) {
      for (let i = 0; i < 35; i++) this.list.push(this.create(Math.random() * 661));
    }
    this.running = true;
    this.frame = requestAnimationFrame(() => this.tick());
  },

  stop() {
    this.running = false;
    cancelAnimationFrame(this.frame);
  },

  tick() {
    if (!this.running) return;
    const g = this.canvas.getContext("2d");
    g.clearRect(0, 0, 384, 661);

    this.list.forEach((ember, i) => {
      ember.y -= ember.speed;
      ember.drift += 0.03;
      ember.x += Math.sin(ember.drift) * 0.3;
      if (ember.y < -5) {
        // Once an ember floats off the top, start a new one at the bottom.
        this.list[i] = this.create(666);
        return;
      }

      // Embers fade out as they rise toward the top.
      const alpha = Math.max(0, Math.min(220, (ember.y / 661) * 260)) / 255;
      g.fillStyle = `rgba(255, ${110 + Math.round(ember.size * 20)}, 30, ${alpha.toFixed(3)})`;
      g.beginPath();
      g.ellipse(ember.x + ember.size / 2, ember.y + ember.size / 2, ember.size / 2, ember.size / 2, 0, 0, Math.PI * 2);
      g.fill();
    });

    this.frame = requestAnimationFrame(() => this.tick());
  },
};

// Results screen: slot-machine reveal

const SPIN_MS = 1600;
const GLOW_MS = 800;

const result = {
  game: null,
  picked: null,     // the real result, decided before the spin starts
  shown: null,      // what the banner is showing right now
  spinning: false,
  start: 0,
  nextSwapAt: 0,
  lockedAt: -1,
  frame: 0,
};

function startSpin() {
  result.picked = rollClass(result.game);
  result.spinning = true;
  result.lockedAt = -1;
  result.nextSwapAt = 0;
  result.start = performance.now();
  $("btn-reroll").disabled = true;

  cancelAnimationFrame(result.frame);
  result.frame = requestAnimationFrame(tick);
}

// Name swaps slow down near the end of the spin.
function swapDelay(elapsed) {
  const progress = elapsed / SPIN_MS;
  return 50 + 300 * progress * progress * progress;
}

function tick(now) {
  const elapsed = now - result.start;

  if (result.spinning) {
    if (elapsed >= SPIN_MS) {
      lockIn();
    } else if (elapsed >= result.nextSwapAt) {
      result.shown = pickDifferent(result.game.classes, result.shown);
      result.nextSwapAt = elapsed + swapDelay(elapsed);
    }
  }

  render();

  // Keep animating until the lock-in glow has faded.
  const glowDone = !result.spinning && now - result.lockedAt > GLOW_MS;
  if (!glowDone) result.frame = requestAnimationFrame(tick);
}

function lockIn() {
  result.spinning = false;
  result.shown = result.picked;
  result.lockedAt = performance.now();
  $("btn-reroll").disabled = false;
  Sound.play("legendary");
}

// Bright flash when the class locks in, then a soft steady glow.
function glowStrength() {
  if (result.lockedAt < 0) return 0;
  const t = (performance.now() - result.lockedAt) / GLOW_MS;
  return t >= 1 ? 0.35 : 1 - 0.65 * t;
}

function render() {
  const glow = glowStrength();
  setText($("status"), result.spinning ? "ROLLING..." : "YOUR CLASS");

  // Class name on the banner
  const nameEl = $("class-name");
  nameEl.classList.toggle("spinning", result.spinning);
  nameEl.classList.toggle("glow", !result.spinning);
  nameEl.style.setProperty("--glow", glow.toFixed(2));
  setText(nameEl, (result.shown || "").toUpperCase());
  fitText(nameEl, result.spinning ? 15 : 16);
  $("banner-glow").style.setProperty("--glow", result.spinning ? 0 : glow.toFixed(2));

  // Every class for this game, with the picked one highlighted
  document.querySelectorAll("#list-cells .cell").forEach((cell) => {
    const name = cell.dataset.cls;
    cell.classList.toggle("picked", !result.spinning && name === result.picked);
    cell.classList.toggle("shown", result.spinning && name === result.shown);
    fitText(cell.querySelector(".txt"), cell.classList.contains("picked") ? 13 : 12);
  });
}

function buildClassList(g) {
  const rows = Math.ceil(g.classes.length / 2);
  const panel = $("class-list");
  panel.style.height = 34 + rows * 22 + "px";
  $("list-cells").style.gridTemplateRows = `repeat(${rows}, 22px)`;

  setText($("list-header"), g.name.toUpperCase() + " CLASSES");
  $("list-cells").replaceChildren(...g.classes.map((name) => {
    const cell = document.createElement("div");
    cell.className = "cell";
    cell.dataset.cls = name;
    const label = document.createElement("div");
    label.className = "txt";
    setText(label, name);
    cell.appendChild(label);
    return cell;
  }));
}

// Wiring: buttons, taps and keyboard

function goTitle() {
  cancelAnimationFrame(result.frame);
  showScreen("screen-title");
}

function goGames() {
  cancelAnimationFrame(result.frame);
  showScreen("screen-games");
}

function goResult(g) {
  result.game = g;
  result.shown = null;
  $("result-logo").src = g.logo;
  buildClassList(g);
  showScreen("screen-result", g.background);
  startSpin();
}

$("btn-start").addEventListener("click", goGames);
$("btn-back").addEventListener("click", goGames);
$("btn-reroll").addEventListener("click", () => {
  Sound.play("freshmeat");
  startSpin();
});

// All five game tiles share one handler - each tile's data-game holds its game's name.
document.querySelectorAll(".game-tile").forEach((tile) => {
  tile.addEventListener("click", () => {
    Sound.play("freshmeat");
    goResult(findGame(tile.dataset.game));
  });
});

// Tap anywhere on the results screen while it spins to skip to the result.
$("screen-result").addEventListener("click", (e) => {
  if (result.spinning && !e.target.closest("button")) {
    lockIn();
    render();
  }
});

// Audio can only start after the first tap or key press.
["pointerdown", "keydown", "touchstart"].forEach((type) => {
  window.addEventListener(type, () => Sound.unlock(), { capture: true, passive: true });
});

window.addEventListener("keydown", (e) => {
  if (currentScreen === "screen-title" && (e.key === "Enter" || e.key === " ")) {
    e.preventDefault();
    goGames();
  } else if (currentScreen === "screen-games" && e.key === "Escape") {
    goTitle();
  } else if (currentScreen === "screen-result") {
    if (e.key === "Escape") {
      goGames();
    } else if (e.key === " " || e.key === "Enter") {
      // Space/Enter skips the spin, or rerolls once it's done.
      e.preventDefault();
      if (result.spinning) {
        lockIn();
        render();
      } else {
        $("btn-reroll").click();
      }
    }
  }
});

// Fit the 384 x 661 stage to the screen

let currentScale = 1;

function fitStage() {
  currentScale = Math.min(window.innerWidth / 384, window.innerHeight / 661, 1.6);
  stage.style.transform = `scale(${currentScale})`;
}

window.addEventListener("resize", fitStage);
window.addEventListener("orientationchange", fitStage);

// Start up

// Preload artwork so nothing pops in when switching screens.
GAMES.forEach((g) => { new Image().src = g.background; });

fitStage();
showScreen("screen-title");

// Service worker (offline + install)
if ("serviceWorker" in navigator) {
  window.addEventListener("load", () => navigator.serviceWorker.register("service-worker.js").catch(() => {}));
}
