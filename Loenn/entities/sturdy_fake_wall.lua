local fakeTilesHelper = require("helpers.fake_tiles")

local sturdyFakeWall = {}

sturdyFakeWall.name = "Sardine7/SturdyFakeWall"
sturdyFakeWall.depth = -13000

function sturdyFakeWall.placements()
    return {
        name = "sturdy_fake_wall",
        data = {
            tiletype = "3",
            width = 8,
            height = 8,
            playTransitionReveal = false,
            playNormalReveal = true,
            permanent = true,
            blendin = true,
            revealSound = "event:/game/general/secret_revealed"
        }
    }
end

sturdyFakeWall.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", true, "tilesFg", {1.0, 1.0, 1.0, 0.7})
sturdyFakeWall.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return sturdyFakeWall
