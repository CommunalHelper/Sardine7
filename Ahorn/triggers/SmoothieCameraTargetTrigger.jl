module Sardine7SmoothieCameraTargetTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/SmoothieCameraTargetTrigger" SmoothieCameraTargetTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, positionMode::String="NoEffect", xLerpStrength::Real=0.0, yLerpStrength::Real=0.0, xOnly::Bool=false, yOnly::Bool=false, deleteFlag::String="")

const placements = Ahorn.PlacementDict(
    "Smoothie Camera Target (Sardine7)" => Ahorn.EntityPlacement(
        SmoothieCameraTargetTrigger,
        "rectangle",
        Dict{String, Any}(),
        function(trigger)
            trigger.data["nodes"] = [(Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"]))]
        end
    )
)

function Ahorn.editingOptions(trigger::SmoothieCameraTargetTrigger)
    return Dict{String, Any}(
        "positionMode" => Maple.trigger_position_modes
    )
end

function Ahorn.nodeLimits(trigger::SmoothieCameraTargetTrigger)
    return 1, 1
end

end