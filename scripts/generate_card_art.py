from __future__ import annotations

from dataclasses import dataclass
import json
from pathlib import Path
import math
import random

from PIL import Image, ImageChops, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
CATALOG_PATH = ROOT / "src" / "Data" / "cards.catalog.json"
SMALL_OUTPUT = ROOT / "src" / "STS2DiscardMod" / "images" / "card_portraits"
BIG_OUTPUT = SMALL_OUTPUT / "big"
SMALL_SIZE = (250, 190)
BIG_SIZE = (606, 852)


@dataclass(frozen=True)
class CardArtSpec:
    name: str
    background: tuple[int, int, int]
    glow: tuple[int, int, int]
    accent: tuple[int, int, int]
    border: tuple[int, int, int]
    particles: tuple[int, int, int]
    motif: str


SPECS = [
    CardArtSpec(
        name="dark_flame_fragment",
        background=(38, 10, 12),
        glow=(255, 132, 76),
        accent=(255, 220, 160),
        border=(255, 154, 82),
        particles=(255, 184, 92),
        motif="fragment",
    ),
    CardArtSpec(
        name="swift_cut",
        background=(8, 47, 61),
        glow=(162, 243, 255),
        accent=(234, 249, 255),
        border=(151, 240, 255),
        particles=(197, 247, 255),
        motif="cut",
    ),
    CardArtSpec(
        name="toxin_record",
        background=(18, 52, 17),
        glow=(143, 255, 99),
        accent=(230, 244, 212),
        border=(164, 255, 111),
        particles=(182, 255, 120),
        motif="toxin",
    ),
    CardArtSpec(
        name="shattered_echo",
        background=(35, 38, 72),
        glow=(207, 214, 255),
        accent=(239, 243, 255),
        border=(208, 214, 255),
        particles=(221, 227, 255),
        motif="echo",
    ),
    CardArtSpec(
        name="ashen_aegis",
        background=(23, 22, 31),
        glow=(255, 148, 94),
        accent=(255, 210, 132),
        border=(255, 177, 87),
        particles=(255, 199, 96),
        motif="shield",
    ),
    CardArtSpec(
        name="crippling_manuscript",
        background=(31, 41, 26),
        glow=(188, 232, 108),
        accent=(235, 241, 204),
        border=(170, 255, 105),
        particles=(199, 255, 128),
        motif="manuscript",
    ),
    CardArtSpec(
        name="ember_volley",
        background=(40, 16, 20),
        glow=(255, 124, 66),
        accent=(255, 223, 112),
        border=(255, 153, 88),
        particles=(255, 187, 93),
        motif="volley",
    ),
    CardArtSpec(
        name="recall_surge",
        background=(13, 31, 53),
        glow=(99, 210, 255),
        accent=(221, 248, 255),
        border=(122, 235, 255),
        particles=(169, 241, 255),
        motif="surge",
    ),
    CardArtSpec(
        name="fading_formula",
        background=(29, 18, 48),
        glow=(164, 121, 255),
        accent=(241, 227, 255),
        border=(183, 140, 255),
        particles=(212, 186, 255),
        motif="formula",
    ),
    CardArtSpec(
        name="final_draft",
        background=(44, 13, 11),
        glow=(255, 104, 74),
        accent=(255, 227, 156),
        border=(255, 131, 78),
        particles=(255, 179, 105),
        motif="draft",
    ),
    CardArtSpec(
        name="dark_momentum",
        background=(8, 18, 42),
        glow=(92, 185, 255),
        accent=(220, 245, 255),
        border=(110, 210, 255),
        particles=(150, 230, 255),
        motif="surge",
    ),
    CardArtSpec(
        name="ash_veil",
        background=(22, 22, 28),
        glow=(210, 215, 230),
        accent=(240, 242, 248),
        border=(200, 205, 225),
        particles=(225, 228, 240),
        motif="shield",
    ),
    CardArtSpec(
        name="void_surge",
        background=(14, 8, 30),
        glow=(175, 80, 255),
        accent=(238, 215, 255),
        border=(185, 96, 255),
        particles=(210, 150, 255),
        motif="fragment",
    ),
]


def lerp(a: int, b: int, factor: float) -> int:
    return int(a + (b - a) * factor)


def scale_box(box: tuple[float, float, float, float], width: int, height: int) -> tuple[int, int, int, int]:
    x1, y1, x2, y2 = box
    return (int(width * x1), int(height * y1), int(width * x2), int(height * y2))


def create_background(spec: CardArtSpec, size: tuple[int, int], rng: random.Random) -> Image.Image:
    width, height = size
    image = Image.new("RGBA", size, spec.background + (255,))
    pixels = image.load()

    for y in range(height):
        vertical = y / max(height - 1, 1)
        curve = 0.5 - 0.5 * math.cos(vertical * math.pi)
        row_color = (
            lerp(spec.background[0], min(255, spec.glow[0] + 12), curve * 0.36),
            lerp(spec.background[1], min(255, spec.glow[1] + 10), curve * 0.24),
            lerp(spec.background[2], min(255, spec.glow[2] + 16), curve * 0.18),
            255,
        )
        for x in range(width):
            horizontal = x / max(width - 1, 1)
            vignette = 0.78 + 0.22 * math.sin(horizontal * math.pi)
            pixels[x, y] = (
                int(row_color[0] * vignette),
                int(row_color[1] * vignette),
                int(row_color[2] * vignette),
                255,
            )

    overlay = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(overlay)

    for _ in range(5):
        ellipse_box = [
            rng.randint(-width // 6, width * 5 // 6),
            rng.randint(-height // 8, height * 6 // 8),
            rng.randint(width // 4, width),
            rng.randint(height // 5, height),
        ]
        ellipse_box[2] += ellipse_box[0]
        ellipse_box[3] += ellipse_box[1]
        fill = spec.glow + (rng.randint(24, 52),)
        draw.ellipse(ellipse_box, fill=fill)

    overlay = overlay.filter(ImageFilter.GaussianBlur(max(10, width // 18)))
    image = Image.alpha_composite(image, overlay)

    streaks = Image.new("RGBA", size, (0, 0, 0, 0))
    streak_draw = ImageDraw.Draw(streaks)
    streak_count = 18 if width > 300 else 10
    for _ in range(streak_count):
        start_x = rng.randint(-width // 4, width)
        start_y = rng.randint(0, height)
        streak_length = rng.randint(width // 8, width // 3)
        angle = rng.uniform(-0.7, -0.35)
        end_x = int(start_x + math.cos(angle) * streak_length)
        end_y = int(start_y + math.sin(angle) * streak_length)
        streak_draw.line((start_x, start_y, end_x, end_y), fill=spec.particles + (85,), width=max(1, width // 95))
    streaks = streaks.filter(ImageFilter.GaussianBlur(max(1, width // 160)))
    image = Image.alpha_composite(image, streaks)

    return image


def add_frame(image: Image.Image, spec: CardArtSpec) -> Image.Image:
    width, height = image.size
    frame = Image.new("RGBA", image.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(frame)
    margin = max(6, width // 28)
    radius = max(16, width // 10)
    line_width = max(2, width // 90)
    draw.rounded_rectangle(
        (margin, margin, width - margin, height - margin),
        radius=radius,
        outline=spec.border + (255,),
        width=line_width,
    )
    glow = frame.filter(ImageFilter.GaussianBlur(max(2, width // 65)))
    glow = ImageChops.screen(glow, glow)
    framed = Image.alpha_composite(image, glow)
    return Image.alpha_composite(framed, frame)


def add_particles(image: Image.Image, spec: CardArtSpec, rng: random.Random) -> Image.Image:
    width, height = image.size
    particles = Image.new("RGBA", image.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(particles)
    count = 72 if width > 300 else 34
    for _ in range(count):
        radius = rng.randint(max(1, width // 240), max(3, width // 60))
        x = rng.randint(0, width)
        y = rng.randint(0, height)
        alpha = rng.randint(135, 235)
        draw.ellipse((x - radius, y - radius, x + radius, y + radius), fill=spec.particles + (alpha,))
    return Image.alpha_composite(image, particles)


def draw_shield(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    shield = [
        (width * 0.5, height * 0.18),
        (width * 0.72, height * 0.28),
        (width * 0.64, height * 0.7),
        (width * 0.5, height * 0.86),
        (width * 0.36, height * 0.7),
        (width * 0.28, height * 0.28),
    ]
    draw.polygon(shield, fill=spec.accent + (240,))
    inner = [
        (width * 0.5, height * 0.26),
        (width * 0.64, height * 0.34),
        (width * 0.58, height * 0.64),
        (width * 0.5, height * 0.74),
        (width * 0.42, height * 0.64),
        (width * 0.36, height * 0.34),
    ]
    draw.polygon(inner, fill=spec.glow + (200,))
    draw.line((width * 0.35, height * 0.33, width * 0.62, height * 0.6), fill=spec.background + (160,), width=max(3, width // 70))
    draw.line((width * 0.58, height * 0.3, width * 0.42, height * 0.68), fill=spec.background + (130,), width=max(2, width // 90))


def draw_fragment(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    shard = [
        (width * 0.5, height * 0.14),
        (width * 0.68, height * 0.38),
        (width * 0.56, height * 0.86),
        (width * 0.34, height * 0.62),
    ]
    draw.polygon(shard, fill=spec.accent + (235,))
    inner = [
        (width * 0.5, height * 0.22),
        (width * 0.6, height * 0.38),
        (width * 0.53, height * 0.72),
        (width * 0.4, height * 0.56),
    ]
    draw.polygon(inner, fill=spec.glow + (205,))
    for offset in (0.0, 0.12, 0.24):
        draw.line(
            (width * (0.18 + offset), height * 0.72, width * (0.3 + offset), height * 0.34),
            fill=spec.glow + (140,),
            width=max(2, width // 95),
        )


def draw_manuscript(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    page = scale_box((0.23, 0.18, 0.77, 0.82), width, height)
    draw.rounded_rectangle(page, radius=max(12, width // 18), fill=spec.accent + (236,))
    draw.rectangle(scale_box((0.39, 0.12, 0.61, 0.26), width, height), fill=tuple(max(0, channel - 45) for channel in spec.accent) + (220,))
    for row in range(4):
        y = 0.34 + row * 0.11
        draw.line((width * 0.32, height * y, width * 0.68, height * y), fill=tuple(max(0, channel - 85) for channel in spec.accent) + (170,), width=max(2, width // 90))
    draw.line((width * 0.38, height * 0.22, width * 0.38, height * 0.72), fill=spec.glow + (130,), width=max(3, width // 80))
    draw.line((width * 0.62, height * 0.26, width * 0.62, height * 0.7), fill=spec.glow + (115,), width=max(2, width // 90))
    for offset in (0.0, 0.08, 0.16):
        draw.arc(scale_box((0.18 + offset, 0.08 + offset * 0.6, 0.82 - offset, 0.92 - offset * 0.6), width, height), start=210, end=325, fill=spec.glow + (155,), width=max(2, width // 95))


def draw_cut(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    blade = [
        (width * 0.18, height * 0.76),
        (width * 0.78, height * 0.2),
        (width * 0.86, height * 0.28),
        (width * 0.26, height * 0.84),
    ]
    draw.polygon(blade, fill=spec.accent + (235,))
    edge = [
        (width * 0.24, height * 0.72),
        (width * 0.78, height * 0.22),
        (width * 0.72, height * 0.34),
        (width * 0.3, height * 0.76),
    ]
    draw.polygon(edge, fill=spec.glow + (210,))
    for idx in range(4):
        y = 0.2 + idx * 0.12
        draw.line(
            (width * (0.56 + idx * 0.06), height * y, width * (0.88 + idx * 0.03), height * (y - 0.12)),
            fill=spec.particles + (120,),
            width=max(1, width // 120),
        )


def draw_toxin(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    page = scale_box((0.2, 0.18, 0.8, 0.86), width, height)
    draw.rounded_rectangle(page, radius=max(12, width // 18), fill=spec.accent + (220,))
    bottle = scale_box((0.38, 0.16, 0.62, 0.52), width, height)
    draw.rounded_rectangle(bottle, radius=max(8, width // 25), fill=tuple(max(0, value - 55) for value in spec.accent) + (230,))
    draw.rectangle(scale_box((0.43, 0.08, 0.57, 0.18), width, height), fill=tuple(max(0, value - 70) for value in spec.accent) + (230,))
    liquid = scale_box((0.4, 0.28, 0.6, 0.48), width, height)
    draw.rounded_rectangle(liquid, radius=max(5, width // 40), fill=spec.glow + (210,))
    for bubble in ((0.46, 0.36), (0.5, 0.42), (0.55, 0.34), (0.53, 0.4)):
        radius = max(2, width // 70)
        cx = width * bubble[0]
        cy = height * bubble[1]
        draw.ellipse((cx - radius, cy - radius, cx + radius, cy + radius), fill=spec.particles + (170,))
    for row in range(3):
        y = 0.62 + row * 0.1
        draw.line((width * 0.33, height * y, width * 0.67, height * y), fill=tuple(max(0, value - 95) for value in spec.accent) + (150,), width=max(2, width // 95))


def draw_echo(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    page = [
        (width * 0.3, height * 0.24),
        (width * 0.76, height * 0.18),
        (width * 0.68, height * 0.82),
        (width * 0.22, height * 0.72),
    ]
    draw.polygon(page, fill=spec.accent + (220,))
    cracks = [
        ((0.34, 0.24), (0.52, 0.44), (0.44, 0.62)),
        ((0.58, 0.2), (0.5, 0.42), (0.63, 0.6)),
        ((0.42, 0.46), (0.3, 0.66)),
    ]
    for crack in cracks:
        points = []
        for x, y in crack:
            points.extend((width * x, height * y))
        draw.line(points, fill=spec.background + (180,), width=max(2, width // 90))
    for offset in (0.0, 0.08, 0.16):
        draw.arc(scale_box((0.12 + offset, 0.12 + offset * 0.4, 0.86 - offset, 0.92 - offset * 0.3), width, height), start=250, end=330, fill=spec.glow + (150,), width=max(2, width // 100))


def draw_volley(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    shards = [
        ((0.28, 0.72), (0.12, 0.2), -0.9),
        ((0.5, 0.58), (0.13, 0.26), -0.65),
        ((0.7, 0.42), (0.1, 0.18), -0.45),
    ]
    for center, size_hint, angle in shards:
        cx = width * center[0]
        cy = height * center[1]
        length = width * size_hint[0]
        breadth = width * size_hint[1] * 0.28
        tip_x = cx + math.cos(angle) * length
        tip_y = cy + math.sin(angle) * length
        side_dx = math.sin(angle) * breadth
        side_dy = -math.cos(angle) * breadth
        polygon = [
            (tip_x, tip_y),
            (cx + side_dx, cy + side_dy),
            (cx - math.cos(angle) * length * 0.24, cy - math.sin(angle) * length * 0.24),
            (cx - side_dx, cy - side_dy),
        ]
        draw.polygon(polygon, fill=spec.glow + (230,))
        draw.line((cx - math.cos(angle) * length * 0.52, cy - math.sin(angle) * length * 0.52, tip_x, tip_y), fill=spec.accent + (165,), width=max(2, width // 95))


def draw_surge(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    stroke = max(3, width // 72)
    for shrink in (0.0, 0.08, 0.16):
        draw.arc(scale_box((0.12 + shrink, 0.2 + shrink * 0.4, 0.88 - shrink, 0.9 - shrink * 0.2), width, height), start=220, end=40, fill=spec.glow + (200,), width=stroke)
    for center_x, center_y, rotation in ((0.36, 0.34, -18), (0.62, 0.48, 15), (0.48, 0.7, -12)):
        card = Image.new("RGBA", layer.size, (0, 0, 0, 0))
        card_draw = ImageDraw.Draw(card)
        card_box = scale_box((center_x - 0.08, center_y - 0.1, center_x + 0.08, center_y + 0.1), width, height)
        card_draw.rounded_rectangle(card_box, radius=max(8, width // 40), fill=spec.accent + (210,))
        card_draw.rectangle(scale_box((center_x - 0.05, center_y - 0.03, center_x + 0.05, center_y + 0.07), width, height), fill=spec.glow + (165,))
        layer.alpha_composite(card.rotate(rotation, resample=Image.Resampling.BICUBIC, center=(width * center_x, height * center_y)))


def draw_formula(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    outer = scale_box((0.22, 0.2, 0.78, 0.76), width, height)
    inner = scale_box((0.3, 0.28, 0.7, 0.68), width, height)
    draw.ellipse(outer, outline=spec.glow + (205,), width=max(3, width // 82))
    draw.ellipse(inner, outline=spec.accent + (180,), width=max(2, width // 110))
    for angle in range(0, 360, 60):
        radians = math.radians(angle)
        x1 = width * 0.5 + math.cos(radians) * width * 0.18
        y1 = height * 0.48 + math.sin(radians) * width * 0.18
        x2 = width * 0.5 + math.cos(radians) * width * 0.28
        y2 = height * 0.48 + math.sin(radians) * width * 0.28
        draw.line((x1, y1, x2, y2), fill=spec.particles + (175,), width=max(2, width // 105))
    draw.polygon([
        (width * 0.5, height * 0.34),
        (width * 0.6, height * 0.5),
        (width * 0.5, height * 0.66),
        (width * 0.4, height * 0.5),
    ], outline=spec.accent + (200,), fill=spec.glow + (90,))
    for idx in range(7):
        x = width * (0.3 + idx * 0.07)
        y = height * (0.78 + (idx % 2) * 0.03)
        size = max(3, width // 72)
        draw.rectangle((x - size, y - size, x + size, y + size), fill=spec.particles + (150,))


def draw_draft(layer: Image.Image, spec: CardArtSpec) -> None:
    width, height = layer.size
    draw = ImageDraw.Draw(layer)
    page = [
        (width * 0.36, height * 0.74),
        (width * 0.68, height * 0.22),
        (width * 0.78, height * 0.26),
        (width * 0.46, height * 0.78),
    ]
    draw.polygon(page, fill=spec.accent + (235,))
    draw.line((width * 0.27, height * 0.78, width * 0.75, height * 0.2), fill=spec.glow + (220,), width=max(8, width // 25))
    draw.line((width * 0.22, height * 0.83, width * 0.7, height * 0.25), fill=spec.accent + (160,), width=max(3, width // 80))
    flame = [
        (width * 0.5, height * 0.18),
        (width * 0.58, height * 0.34),
        (width * 0.55, height * 0.48),
        (width * 0.63, height * 0.62),
        (width * 0.5, height * 0.7),
        (width * 0.41, height * 0.56),
        (width * 0.43, height * 0.42),
        (width * 0.35, height * 0.3),
    ]
    draw.polygon(flame, fill=spec.glow + (180,))


def load_catalog_asset_names() -> list[str]:
    with CATALOG_PATH.open("r", encoding="utf-8") as handle:
        data = json.load(handle)

    # Exclude Power model entries (those with smartDescription in any locale) — they don't get card art
    result = []
    for card in data["cards"]:
        localization = card.get("localization", {})
        is_power = any(
            "smartDescription" in locale_data
            for locale_data in localization.values()
        )
        if not is_power:
            result.append(card["assetName"])
    return result


def validate_specs() -> None:
    catalog_assets = load_catalog_asset_names()
    spec_names = [spec.name for spec in SPECS]

    missing = sorted(set(catalog_assets) - set(spec_names))
    extras = sorted(set(spec_names) - set(catalog_assets))
    duplicates = sorted({name for name in spec_names if spec_names.count(name) > 1})

    if missing or extras or duplicates:
        raise ValueError(
            "Card art specs are out of sync with cards.catalog.json: "
            f"missing={missing}; extras={extras}; duplicates={duplicates}")


def build_motif(spec: CardArtSpec, size: tuple[int, int]) -> Image.Image:
    motif = Image.new("RGBA", size, (0, 0, 0, 0))
    if spec.motif == "fragment":
        draw_fragment(motif, spec)
    elif spec.motif == "cut":
        draw_cut(motif, spec)
    elif spec.motif == "toxin":
        draw_toxin(motif, spec)
    elif spec.motif == "echo":
        draw_echo(motif, spec)
    elif spec.motif == "shield":
        draw_shield(motif, spec)
    elif spec.motif == "manuscript":
        draw_manuscript(motif, spec)
    elif spec.motif == "volley":
        draw_volley(motif, spec)
    elif spec.motif == "surge":
        draw_surge(motif, spec)
    elif spec.motif == "formula":
        draw_formula(motif, spec)
    elif spec.motif == "draft":
        draw_draft(motif, spec)
    else:
        raise ValueError(f"Unknown motif: {spec.motif}")

    glow = motif.filter(ImageFilter.GaussianBlur(max(6, size[0] // 22)))
    glow = ImageChops.screen(glow, glow)
    return Image.alpha_composite(glow, motif)


def render_card_art(spec: CardArtSpec, size: tuple[int, int]) -> Image.Image:
    rng = random.Random(f"{spec.name}:{size[0]}x{size[1]}")
    image = create_background(spec, size, rng)
    motif = build_motif(spec, size)
    image = Image.alpha_composite(image, motif)
    image = add_particles(image, spec, rng)
    image = add_frame(image, spec)
    return image


def save_art(spec: CardArtSpec, size: tuple[int, int], output_dir: Path) -> None:
    output_dir.mkdir(parents=True, exist_ok=True)
    image = render_card_art(spec, size)
    image.save(output_dir / f"{spec.name}.png")


def main() -> None:
    validate_specs()

    for spec in SPECS:
        save_art(spec, SMALL_SIZE, SMALL_OUTPUT)
        save_art(spec, BIG_SIZE, BIG_OUTPUT)
        print(f"generated {spec.name}")


if __name__ == "__main__":
    main()