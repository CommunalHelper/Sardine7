local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local chainZipMover = {}

local altMoonBlockTex = "objects/Sardine7/ChainMover/block"
local themeTextures = {
    normal = {
        nodeCog = "objects/zipmover/cog",
        lights = "objects/zipmover/light01",
        block = "objects/zipmover/block",
        innerCogs = "objects/zipmover/innercog"
    },
    moon = {
        nodeCog = "objects/zipmover/moon/cog",
        lights = "objects/zipmover/moon/light01",
        block = "objects/zipmover/moon/block",
        innerCogs = "objects/zipmover/moon/innercog"
    }
}

local blockNinePatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local centerColor = {0, 0, 0}
local ropeColor = {102 / 255, 57 / 255, 49 / 255}

local themes = {
    "Normal", "Moon"
}

chainZipMover.name = "Sardine7/ChainMover"
chainZipMover.depth = -9999
chainZipMover.nodeVisibility = "never"
chainZipMover.nodeLimits = {2, -1}
chainZipMover.minimumSize = {16, 16}
chainZipMover.fieldInformation = {
    theme = {
        options = themes,
        editable = false
    }
}
chainZipMover.placements = {}

for i, theme in ipairs(themes) do
    chainZipMover.placements[i] = {
        name = string.lower(theme),
        data = {
            width = 16,
            height = 16,
            theme = theme,
            largerMoonBorder = true
        }
    }
end

local function addNodeSprites(sprites, entity, cogTexture, centerX, centerY, centerNodeX, centerNodeY)
    local nodeCogSprite = drawableSprite.fromTexture(cogTexture, entity)

    nodeCogSprite:setPosition(centerNodeX, centerNodeY)
    nodeCogSprite:setJustification(0.5, 0.5)

    local points = {centerX, centerY, centerNodeX, centerNodeY}
    local leftLine = drawableLine.fromPoints(points, ropeColor, 1)
    local rightLine = drawableLine.fromPoints(points, ropeColor, 1)

    leftLine:setOffset(0, 4.5)
    rightLine:setOffset(0, -4.5)

    leftLine.depth = 5000
    rightLine.depth = 5000

    for _, sprite in ipairs(leftLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    for _, sprite in ipairs(rightLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, nodeCogSprite)
end

local function addBlockSprites(sprites, entity, blockTexture, lightsTexture, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, centerColor)

    local frameNinePatch = drawableNinePatch.fromTexture(blockTexture, blockNinePatchOptions, x, y, width, height)
    local frameSprites = frameNinePatch:getDrawableSprite()

    local lightsSprite = drawableSprite.fromTexture(lightsTexture, entity)

    lightsSprite:addPosition(math.floor(width / 2), 0)
    lightsSprite:setJustification(0.5, 0.0)

    table.insert(sprites, rectangle:getDrawableSprite())

    for _, sprite in ipairs(frameSprites) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, lightsSprite)
end

function chainZipMover.sprite(room, entity)
    local sprites = {}

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 16, entity.height or 16
    
    local theme = string.lower(entity.theme or "normal")
    local themeData = themeTextures[theme] or themeTextures["normal"]
    local blockTex = themeData.block
    if theme == "Moon" and entity.largerMoonBorder then
        blockTex = altMoonBlockTex
    end
    
    addBlockSprites(sprites, entity, blockTex, themeData.lights, x, y, width, height)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)
    
    for i, node in ipairs(nodes) do
        local nodeX, nodeY = nodes[i].x, nodes[i].y
        local prevX = (i == 1) and x or nodes[i - 1].x
        local prevY = (i == 1) and y or nodes[i - 1].y
        local centerX, centerY = prevX + halfWidth, prevY + halfHeight
        local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight
        addNodeSprites(sprites, entity, themeData.nodeCog, centerX, centerY, centerNodeX, centerNodeY)
    end    

    return sprites
end

function chainZipMover.selection(room, entity)
    local nodeRectangles = {}
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8
    local mainRectangle = utils.rectangle(x, y, width, height)
    
    local nodes = entity.nodes or {{x = 0, y = 0}}
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)
    
    local theme = string.lower(entity.theme or "normal")
    local themeData = themeTextures[theme] or themeTextures["normal"]

    local cogSprite = drawableSprite.fromTexture(themeData.nodeCog, entity)
    local cogWidth, cogHeight = cogSprite.meta.width, cogSprite.meta.height
    
    for i, node in ipairs(nodes) do
        local nodeX, nodeY = nodes[i].x, nodes[i].y
        local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight
        local nodeRectangle = utils.rectangle(centerNodeX - math.floor(cogWidth / 2), centerNodeY - math.floor(cogHeight / 2), cogWidth, cogHeight)
        table.insert(nodeRectangles, nodeRectangle)
    end
    
    return mainRectangle, nodeRectangles
end

return chainZipMover
