import { Particle } from "./Particle";

export function spawnParticles(
  canvas: HTMLCanvasElement,
  colors: string[],
  particles: Particle[]
) {
  const centerX = canvas.width / 2;
  const centerY = canvas.height / 2;

  for (let i = 0; i < 5; i++) {
    const color = colors[Math.floor(Math.random() * colors.length)];
    particles.push(new Particle(centerX, centerY, color));
  }
}
