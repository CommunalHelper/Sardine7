module Sardine7SokobanBlock

using ..Ahorn, Maple

@mapdef Entity "Sardine7/SokobanBlock" SokobanBlock(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Sokoban Block (Both, Sardine7)" => Ahorn.EntityPlacement(
        SokobanBlock,
        "rectangle"
    ),
    "Sokoban Block (Vertical, Sardine7)" => Ahorn.EntityPlacement(
        SokobanBlock,
        "rectangle",
        Dict{String, Any}(
            "axes" => "vertical"
        )
    ),
    "Sokoban Block (Horizontal, Sardine7)" => Ahorn.EntityPlacement(
        SokobanBlock,
        "rectangle",
        Dict{String, Any}(
            "axes" => "horizontal"
        )
    ),
)

frameImage = Dict{String, String}(
    "none" => "objects/Sardine7/SokobanBlock/block00",
    "horizontal" => "objects/Sardine7/SokobanBlock/block01",
    "vertical" => "objects/Sardine7/SokobanBlock/block02",
    "both" => "objects/Sardine7/SokobanBlock/block03"
)

smallFace = "objects/Sardine7/SokobanBlock/idle_face"
giantFace = "objects/Sardine7/SokobanBlock/giant_block00"

kevinColor = (98, 34, 43) ./ 255

Ahorn.editingOptions(entity::SokobanBlock) = Dict{String, Any}(
    "axes" => Maple.kevin_axes
)

Ahorn.minimumSize(entity::SokobanBlock) = 24, 24
Ahorn.resizable(entity::SokobanBlock) = true, true

Ahorn.selection(entity::SokobanBlock) = Ahorn.getEntityRectangle(entity)

# what even is randomness
function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::SokobanBlock, room::Maple.Room)
    axes = lowercase(get(entity.data, "axes", "both"))
    chillout = get(entity.data, "chillout", false)

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    giant = height >= 48 && width >= 48 && chillout
    face = giant ? giantFace : smallFace
    frame = frameImage[lowercase(axes)]
    faceSprite = Ahorn.getSprite(face, "Gameplay")

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    Ahorn.drawRectangle(ctx, 2, 2, width - 4, height - 4, kevinColor)
    Ahorn.drawImage(ctx, faceSprite, div(width - faceSprite.width, 2), div(height - faceSprite.height, 2))

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, height - 8, 8, 24, 8, 8)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, frame, width - 8, (i - 1) * 8, 24, 8, 8, 8)
    end

    Ahorn.drawImage(ctx, frame, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, 0, 24, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, 0, height - 8, 0, 24, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, height - 8, 24, 24, 8, 8)
end

end