export class Particle {
    x: number;
    y: number;
    speed: number;
    angle: number;
    accel: number;
    life: number;
    decay: number;
    color: string;
  
    constructor(x: number, y: number, color: string) {
      this.x = x;
      this.y = y;
      this.speed = Math.random() * 2 + 0.5;
      this.angle = Math.random() * Math.PI * 2;
      this.accel = 0.01;
      this.life = Math.random() * 50 + 50;
      this.decay = 0.98;
      this.color = color;
    }
  
    update() {
      this.speed *= this.decay;
      this.x += Math.cos(this.angle) * this.speed;
      this.y += Math.sin(this.angle) * this.speed;
      this.life -= 1;
    }
  
    draw(ctx: CanvasRenderingContext2D) {
      ctx.beginPath();
      ctx.arc(this.x, this.y, 2, 0, Math.PI * 2);
      ctx.fillStyle = this.color;
      ctx.fill();
    }
  }
  