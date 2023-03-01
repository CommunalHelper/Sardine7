module Sardine7AmbienceTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/AmbienceTrigger" AmbienceTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, track::String="", resetOnLeave::Bool=false)

const placements = Ahorn.PlacementDict(
    "Ambience (Sardine7)" => Ahorn.EntityPlacement(
        AmbienceTrigger,
        "rectangle"
    )
)

function Ahorn.editingOptions(trigger::AmbienceTrigger)
    return Dict{String, Any}(
        "track" => sort(collect(keys(Maple.AmbientSounds.sounds)))
    )
end

end