import os
from PIL import Image, ImageDraw

OUT_DIR = r"E:\Users\mrcto\Desktop\UNITY\Eh-Oh-Mosquitoes\Assets\Ressources\UI\Refont UI\Generated"
os.makedirs(OUT_DIR, exist_ok=True)

META_TEMPLATE = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 0
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 0
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: {bx}, y: {by}, z: {bz}, w: {bw}}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: {guid}
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
  spritePackingTag: 
  pSDRemoveMatte: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

def save_with_meta(img, filename, guid, border=(0, 0, 0, 0)):
    path = os.path.join(OUT_DIR, filename)
    img.save(path, "PNG")
    meta_path = path + ".meta"
    bx, by, bz, bw = border
    with open(meta_path, "w", encoding="utf-8", newline="\n") as f:
        f.write(META_TEMPLATE.format(guid=guid, bx=bx, by=by, bz=bz, bw=bw))
    print(f"Generated {filename} + .meta (guid={guid})")

# Also create folder .meta if missing
folder_meta = OUT_DIR + ".meta"
if not os.path.exists(folder_meta):
    with open(folder_meta, "w", encoding="utf-8", newline="\n") as f:
        f.write("""fileFormatVersion: 2
guid: a0000000000000000000000000000099
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
""")

# 1. Ndop Ribbon (1024 x 36)
def make_ndop_ribbon():
    scale = 2
    w, h = 1024 * scale, 36 * scale
    img = Image.new("RGBA", (w, h), (35, 52, 27, 255)) # Olive dark #23341B
    draw = ImageDraw.Draw(img)
    gold = (245, 184, 46, 255)
    step = 24 * scale
    for x in range(-h, w + h, step):
        draw.line([(x, 0), (x + h, h)], fill=gold, width=3 * scale)
        draw.line([(x, h), (x + h, 0)], fill=gold, width=3 * scale)
    # Top and bottom crisp hairline
    draw.line([(0, 0), (w, 0)], fill=(245, 184, 46, 220), width=2 * scale)
    draw.line([(0, h - 1), (w, h - 1)], fill=(245, 184, 46, 220), width=2 * scale)
    img = img.resize((1024, 36), Image.Resampling.LANCZOS)
    save_with_meta(img, "ndop_ribbon.png", "a1010101010101010101010101010101")

# 2. Rounded Card Fill (128x128, radius=24, 9-slice border=30)
def make_card_rounded():
    s = 4
    size = 128 * s
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle([2*s, 2*s, size - 2*s - 1, size - 2*s - 1], radius=24*s, fill=(255, 255, 255, 255))
    img = img.resize((128, 128), Image.Resampling.LANCZOS)
    save_with_meta(img, "ui_card_rounded.png", "a2020202020202020202020202020202", border=(30, 30, 30, 30))

# 3. Rounded Card Border (128x128, radius=24, stroke=3px, 9-slice border=30)
def make_card_border():
    s = 4
    size = 128 * s
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle([2*s, 2*s, size - 2*s - 1, size - 2*s - 1], radius=24*s, outline=(255, 255, 255, 255), width=3*s)
    img = img.resize((128, 128), Image.Resampling.LANCZOS)
    save_with_meta(img, "ui_card_border.png", "a3030303030303030303030303030303", border=(30, 30, 30, 30))

# 4. Pill / Capsule (128x64, radius=30, 9-slice border=32,16,32,16)
def make_pill():
    s = 4
    w, h = 128 * s, 64 * s
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle([1*s, 1*s, w - 1*s - 1, h - 1*s - 1], radius=31*s, fill=(255, 255, 255, 255))
    img = img.resize((128, 64), Image.Resampling.LANCZOS)
    save_with_meta(img, "ui_pill_rounded.png", "a4040404040404040404040404040404", border=(32, 16, 32, 16))

# 5. Circle (128x128)
def make_circle():
    s = 4
    size = 128 * s
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.ellipse([2*s, 2*s, size - 2*s - 1, size - 2*s - 1], fill=(255, 255, 255, 255))
    img = img.resize((128, 128), Image.Resampling.LANCZOS)
    save_with_meta(img, "ui_circle.png", "a5050505050505050505050505050505")

make_ndop_ribbon()
make_card_rounded()
make_card_border()
make_pill()
make_circle()
print("All 5 UI sprites generated cleanly!")
