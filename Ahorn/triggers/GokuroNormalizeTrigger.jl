module Sardine7GokuroNormalizeTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/GokuroNormalizeTrigger" GokuroNormalizeTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, count::Integer=1)

const placements = Ahorn.PlacementDict(
    "Gokuro (Normalize, Sardine7)" => Ahorn.EntityPlacement(
        GokuroNormalizeTrigger,
        "rectangle"
    )
)

end