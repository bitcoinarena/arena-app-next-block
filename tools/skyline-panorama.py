#!/usr/bin/env python3
# kanban #968: the header image of the website redrawn as a 360-degree panorama for Next Block's sky: the bitcoin sun over a
# skyline of price candles. 8192 x 1536 px, 22.76 px per degree, from the horizon (bottom row) to 67.5 degrees of elevation.
# Prints an SVG; a browser screenshot of it at that size is unity/Assets/Resources/skyline-panorama.png.
import math, random
W, H = 8192, 1536
PX = W / 360.0
r = random.Random(968)
out = [f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H}" viewBox="0 0 {W} {H}">', '<defs>',
  '<linearGradient id="sky" x1="0" y1="0" x2="0" y2="1">'
  '<stop offset="0" stop-color="#140b1f"/><stop offset=".45" stop-color="#22102a"/><stop offset=".72" stop-color="#3b1338"/>'
  '<stop offset=".9" stop-color="#8a2a3c"/><stop offset="1" stop-color="#e2672a"/></linearGradient>',
  '<radialGradient id="sun" cx="50%" cy="38%" r="62%"><stop offset="0" stop-color="#fff6d6"/><stop offset=".35" stop-color="#ffd166"/>'
  '<stop offset=".75" stop-color="#f7931a"/><stop offset="1" stop-color="#e2562a"/></radialGradient>',
  '<radialGradient id="halo"><stop offset="0" stop-color="#ffb347" stop-opacity=".55"/><stop offset=".45" stop-color="#f7931a" stop-opacity=".18"/>'
  '<stop offset="1" stop-color="#f7931a" stop-opacity="0"/></radialGradient>',
  '</defs>', f'<rect width="{W}" height="{H}" fill="url(#sky)"/>']
# stars, none near the top edge's seam with the plain sky above it
for _ in range(1500):
    x, y = r.uniform(0, W), r.uniform(40, H - 330)
    a = r.uniform(.25, .95) * min(1, (H - 330 - y) / 200)
    out.append(f'<circle cx="{x:.1f}" cy="{y:.1f}" r="{r.choice([1.2, 1.5, 1.5, 2, 2.6]):.1f}" fill="#fff" opacity="{a:.2f}"/>')
# the sun, at +X: the road's end, where the miner stands
sx, sy, sr = W * .75, H - 200, 300
out.append(f'<circle cx="{sx}" cy="{sy}" r="{sr * 2.7}" fill="url(#halo)"/>')
out.append(f'<circle cx="{sx}" cy="{sy}" r="{sr}" fill="url(#sun)"/>')
for i, (dy, h) in enumerate([(70, 7), (105, 9), (140, 11), (172, 14)]):
    out.append(f'<rect x="{sx - sr}" y="{sy + dy}" width="{2 * sr}" height="{h}" fill="#3b1338" opacity=".75"/>')
# mountains that close at the seam
for f, col, base, amp in [(3, '#5a1d43', 92, 46), (5, '#3a1232', 66, 34)]:
    p = [(0, H)]
    phase = r.uniform(0, 6.28)
    for x in range(0, W + 1, 16):
        t = x / W * 2 * math.pi
        y = base + amp * (.6 * math.sin(f * t + phase) + .4 * math.sin(2 * f * t + 1.7 * phase) + .25 * math.sin(7 * f * t))
        p.append((x, H - max(0, y)))
    p.append((W, H))
    out.append('<polygon fill="%s" points="%s"/>' % (col, ' '.join(f'{a},{b:.0f}' for a, b in p)))
# the candles, all the way round
x = 0
while x < W - 46:
    w = r.choice([34, 40, 46, 52, 60])
    h = r.choice([70, 90, 110, 130, 150, 170, 200, 235, 270])
    top = H - h
    up = r.random() < .55
    cap = '#2fd38a' if up else '#ff5d5d'
    out.append(f'<rect x="{x + w / 2 - 2.5}" y="{top - r.randint(20, 70)}" width="5" height="{h}" fill="#1d0e1f"/>')
    out.append(f'<rect x="{x}" y="{top}" width="{w}" height="{h}" fill="#1d0e1f"/>')
    out.append(f'<rect x="{x}" y="{top}" width="{w}" height="6" fill="{cap}"/>')
    y = top + 22
    while y < H - 14:
        if r.random() < .34:
            out.append(f'<rect x="{x + w / 2 - 5}" y="{y}" width="10" height="13" fill="#ffc46b"/>')
        y += r.choice([26, 30, 36])
    x += w + r.choice([18, 26, 36, 50, 70])
out.append(f'<rect y="{H - 3}" width="{W}" height="3" fill="#ffb347"/>')
out.append('</svg>')
print('\n'.join(out))
