module Sardine7BunnyBlockField

using ..Ahorn, Maple

@mapdef Entity "Sardine7/BunnyBlockField" BunnyBlockField(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Bunny Blockfield (Sardine7)" => Ahorn.EntityPlacement(
        BunnyBlockField,
		"rectangle"
    )
)

Ahorn.minimumSize(entity::BunnyBlockField) = 8, 8
Ahorn.resizable(entity::BunnyBlockField) = true, true

Ahorn.selection(entity::BunnyBlockField) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BunnyBlockField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.4, 0.4, 1.0, 0.4), (0.4, 0.4, 1.0, 1.0))
end

end