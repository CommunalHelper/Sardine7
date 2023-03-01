module Sardine7Beeker

using ..Ahorn, Maple

@mapdef Entity "Sardine7/Beeker" Beeker(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Beeker (Sardine7)" => Ahorn.EntityPlacement(
        Beeker,
        "point",
        Dict{String, Any}(),
        function(entity)
            entity.data["nodes"] = [(Int(entity.data["x"]) + 32, Int(entity.data["y"]))]
        end
    )
)

Ahorn.nodeLimits(entity::Beeker) = 1, -1

function Ahorn.selection(entity::Beeker)
    nodes = get(entity.data, "nodes", ())
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(sprite, x, y)]
    
    for node in nodes
        nx, ny = node

        push!(res, Ahorn.getSpriteRectangle(sprite, nx, ny))
    end

    return res
end

sprite = "characters/monsters/predator65.png"

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::Beeker)
    px, py = Ahorn.position(entity)

    for node in get(entity.data, "nodes", ())
        nx, ny = Int.(node)

        Ahorn.drawArrow(ctx, px, py, nx, ny, Ahorn.colors.selection_selected_fc, headLength=6)
        Ahorn.drawSprite(ctx, sprite, nx, ny)

        px, py = nx, ny
    end
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Beeker, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end