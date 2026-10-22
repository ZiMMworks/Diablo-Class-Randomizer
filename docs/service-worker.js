// Offline cache. Bump CACHE_VERSION when files change.

const CACHE_VERSION = "diablo-randomizer-v2";

const FILES = [
  "./",
  "index.html",
  "style.css",
  "app.js",
  "manifest.json",
  "assets/images/diablo1.jpg",
  "assets/images/diablo2.jpg",
  "assets/images/diablo3.jpg",
  "assets/images/diablo4.jpg",
  "assets/images/diabloimmortal.jpg",
  "assets/images/diablotitle.png",
  "assets/images/gamepage.png",
  "assets/images/randomclass.png",
  "assets/images/result_diablo1.png",
  "assets/images/result_diablo2.png",
  "assets/images/result_diablo3.png",
  "assets/images/result_diablo4.png",
  "assets/images/result_diabloimmortal.png",
  "assets/icons/icon-180.png",
  "assets/icons/icon-192.png",
  "assets/icons/icon-512.png",
  "assets/icons/icon-maskable-512.png",
  "assets/sounds/freshmeat.mp3",
  "assets/sounds/legendary.mp3",
  "assets/fonts/diabloheavy.ttf"
];

self.addEventListener("install", (event) => {
  event.waitUntil(caches.open(CACHE_VERSION).then((cache) => cache.addAll(FILES)));
  self.skipWaiting();
});

self.addEventListener("activate", (event) => {
  // Delete caches from older versions.
  event.waitUntil(
    caches.keys().then((keys) =>
      Promise.all(keys.filter((key) => key !== CACHE_VERSION).map((key) => caches.delete(key)))
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", (event) => {
  if (event.request.method !== "GET") return;
  event.respondWith(
    caches.match(event.request).then((cached) => cached || fetch(event.request))
  );
});
