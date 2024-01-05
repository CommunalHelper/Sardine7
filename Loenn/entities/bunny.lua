local drawableSprite = require("structs.drawable_sprite")

local bunny = {}

bunny.name = "Sardine7/Bunny"
bunny.depth = -9999
bunny.placements = {
    name = "bunny"
}

local offsetY = -4
local texture = "scenery/Sardine7/bunny/idle00"

function bunny.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(texture, entity)

    sprite.y += offsetY

    return sprite
end

return bunny
