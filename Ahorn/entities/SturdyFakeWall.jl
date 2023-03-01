module Sardine7SturdyFakeWall

using ..Ahorn, Maple

@mapdef Entity "Sardine7/SturdyFakeWall" SturdyFakeWall(x::Integer, y::Integer, playTransitionReveal::Bool=false, playNormalReveal::Bool=true, permanent::Bool=true, blendin::Bool=true, revealSound::String="event:/game/general/secret_revealed", tiletype::String="3")

const placements = Ahorn.PlacementDict(
    "Sturdy Fake Wall (Sardine7)" => Ahorn.EntityPlacement(
        SturdyFakeWall,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::SturdyFakeWall) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::SturdyFakeWall) = 8, 8
Ahorn.resizable(entity::SturdyFakeWall) = true, true

Ahorn.selection(entity::SturdyFakeWall) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::SturdyFakeWall, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, alpha=0.7)

end