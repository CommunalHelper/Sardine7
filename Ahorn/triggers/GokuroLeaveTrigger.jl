module Sardine7GokuroLeaveTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/GokuroLeaveTrigger" GokuroLeaveTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, count::Integer=1)

const placements = Ahorn.PlacementDict(
    "Gokuro (Leave, Sardine7)" => Ahorn.EntityPlacement(
        GokuroLeaveTrigger,
        "rectangle"
    )
)

end