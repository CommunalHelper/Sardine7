module Sardine7SecondTextBoxTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/SecondTextboxTrigger" SecondTextBoxTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, mode::String="OnPlayerEnter", dialog_id::String="", only_once::Bool=true, death_count::Integer=-1, lifespan::String="3.0")

const placements = Ahorn.PlacementDict(
    "Second Textbox (Sardine7)" => Ahorn.EntityPlacement(
        SecondTextBoxTrigger,
        "rectangle"
    )
)

function Ahorn.editingOptions(trigger::SecondTextBoxTrigger)
    return Dict{String, Any}(
        "mode" => Maple.mini_textbox_trigger_modes
    )
end

end