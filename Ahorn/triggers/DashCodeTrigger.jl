module Sardine7DashCodeTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/DashCodeTrigger" DashCodeTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, code::String="U,L,DR,UR,L,UL", flag::String="", flagValue::Bool=true, resetOnLeave::Bool=false, disableOnLeave::Bool=true)

const placements = Ahorn.PlacementDict(
    "Dash Code (Sardine7)" => Ahorn.EntityPlacement(
        DashCodeTrigger,
        "rectangle"
    )
)

end