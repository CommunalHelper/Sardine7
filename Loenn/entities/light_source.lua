local drawableSprite = require("structs.drawable_sprite")

local lightSource = {}

lightSource.name = "Sardine7/LightSource"

lightSource.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true
    },
    alpha = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    },
    startFade = {
        fieldType = "integer",
        minimumValue = 1
    },
    endFade = {
        fieldType = "integer",
        minimumValue = 1
    },
}

lightSource.fieldOrder = {
    "x", "y", "alpha", "color", "startFade", "endFade"
}

lightSource.placements = {
    name = "light_source",
    data = {
      alpha = 1.0,
      startFade = 24,
      endFade = 48,
      color = "White"
    }
}

local texture = "objects/Sardine7/LightSource/hanginglamp_extended"

function lightSource.sprite(room, entity)
    local sprite = drawableSprite.fromTexture(texture, entity)
    sprite:setAlpha(0.5)
    
    return sprite
end

return lightSource
