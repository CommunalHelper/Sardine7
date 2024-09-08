local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")

local sokobanBlock = {}

local axesOptions = {
    Both = "both",
    Vertical = "vertical",
    Horizontal = "horizontal"
}

sokobanBlock.name = "Sardine7/SokobanBlock"
sokobanBlock.depth = 0
sokobanBlock.minimumSize = {24, 24}
sokobanBlock.fieldInformation = {
    axes = {
        options = axesOptions,
        editable = false
    }
}
sokobanBlock.placements = {}

for _, axis in pairs(axesOptions) do
    table.insert(sokobanBlock.placements, {
        name = axis,
        data = {
            width = 24,
            height = 24,
            axes = axis
        }
    })
end

local frameTextures = {
    none = "objects/Sardine7/SokobanBlock/block00",
    horizontal = "objects/Sardine7/SokobanBlock/block01",
    vertical = "objects/Sardine7/SokobanBlock/block02",
    both = "objects/Sardine7/SokobanBlock/block03"
}

local ninePatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local blockColor = {98 / 255, 34 / 255, 43 / 255}
local smallFaceTexture = "objects/Sardine7/SokobanBlock/idle_face"
local giantFaceTexture = "objects/Sardine7/SokobanBlock/giant_block00"

function sokobanBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local axes = entity.axes or "both"
    local chillout = entity.chillout

    local giant = height >= 48 and width >= 48 and chillout
    local faceTexture = giant and giantFaceTexture or smallFaceTexture

    local frameTexture = frameTextures[axes] or frameTextures["both"]
    local ninePatch = drawableNinePatch.fromTexture(frameTexture, ninePatchOptions, x, y, width, height)

    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, blockColor)
    local faceSprite = drawableSprite.fromTexture(faceTexture, entity)

    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))

    local sprites = ninePatch:getDrawableSprite()

    table.insert(sprites, 1, rectangle:getDrawableSprite())
    table.insert(sprites, 2, faceSprite)

    return sprites
end

function sokobanBlock.rotate(room, entity, direction)
    local axes = (entity.axes or ""):lower()

    if axes == "horizontal" then
        entity.axes = "vertical"

    elseif axes == "vertical" then
        entity.axes = "horizontal"
    end

    return true
end

return sokobanBlock
