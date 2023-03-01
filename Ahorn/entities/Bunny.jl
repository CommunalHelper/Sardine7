module Sardine7Bunny

using ..Ahorn, Maple

@mapdef Entity "Sardine7/Bunny" Bunny(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Bunny (Sardine7)" => Ahorn.EntityPlacement(
        Bunny
    )
)

const sprite = "scenery/Sardine7/bunny/idle00.png"

function Ahorn.selection(entity::Bunny)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y, jx=0.5, jy=1.0)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Bunny, room::Maple.Room)
    Ahorn.drawSprite(ctx, sprite, 0, 0, jx=0.5, jy=1.0)
end

end