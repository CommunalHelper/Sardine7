module Sardine7LightSource

using ..Ahorn, Maple

@mapdef Entity "Sardine7/LightSource" LightSource(x::Integer, y::Integer, alpha::Number=1.0, startFade::Number=24.0, endFade::Number=48.0, color::String="White")

const colors = sort(collect(keys(Ahorn.XNAColors.colors)))

const placements = Ahorn.PlacementDict(
	"Light Source (Sardine7)" => Ahorn.EntityPlacement(
		LightSource
	)
)

Ahorn.editingOptions(entity::LightSource) = Dict{String,Any}(
	"color" => colors
)

function Ahorn.selection(entity::LightSource)
	x, y = Ahorn.position(entity)

	return Ahorn.Rectangle(x, y, 8, 8)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::LightSource, room::Maple.Room)
	x, y = Ahorn.position(entity)
	sprite = Ahorn.getTextureSprite("objects/Sardine7/LightSource/hanginglamp_extended", "Gameplay")
	
	Ahorn.drawImage(ctx, sprite, x, y, 0, 0, 8, 8, alpha=0.5)
end

end