module Sardine7ChainMover

using ..Ahorn, Maple

@mapdef Entity "Sardine7/ChainMover" ChainMover(x::Integer, y::Integer, largerMoonBorder::Bool=true)

const placements = Ahorn.PlacementDict(
    "Chain Mover ($(uppercasefirst(theme))) (Sardine7)" => Ahorn.EntityPlacement(
        ChainMover,
        "rectangle",
        Dict{String, Any}(
            "theme" => theme
        ),
        function(entity)
            entity.data["nodes"] = [(Int(entity.data["x"]) + Int(entity.data["width"]), Int(entity.data["y"])), (Int(entity.data["x"]) + Int(entity.data["width"]) * 2, Int(entity.data["y"]))]
        end
    ) for theme in Maple.zip_mover_themes
)

Ahorn.editingOptions(entity::ChainMover) = Dict{String, Any}(
    "theme" => Maple.zip_mover_themes
)

Ahorn.nodeLimits(entity::ChainMover) = -1, -1
Ahorn.minimumSize(entity::ChainMover) = 16, 16
Ahorn.resizable(entity::ChainMover) = true, true

function Ahorn.selection(entity::ChainMover)
	nodes = get(entity.data, "nodes", ())
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))
	
	res = Ahorn.Rectangle[Ahorn.Rectangle(x, y, width, height)]
	
	for node in nodes
        nx, ny = Int.(node)

        push!(res, Ahorn.Rectangle(nx + floor(Int, width / 2) - 5, ny + floor(Int, height / 2) - 5, 10, 10))
    end

    return res
end

function getTextures(entity::ChainMover)
    theme = lowercase(get(entity, "theme", "normal"))
	border = get(entity.data, "largerMoonBorder", true)
    
    if theme == "moon"
		if border
			return "objects/Sardine7/ChainMover/block", "objects/zipmover/moon/light01", "objects/zipmover/moon/cog"
        end
		return "objects/zipmover/moon/block", "objects/zipmover/moon/light01", "objects/zipmover/moon/cog"
    end

    return "objects/zipmover/block", "objects/zipmover/light01", "objects/zipmover/cog"
end

ropeColor = (102, 57, 49) ./ 255

function renderChainMover(ctx::Ahorn.Cairo.CairoContext, entity::ChainMover)
	width = Int(get(entity.data, "width", 32))
	height = Int(get(entity.data, "height", 32))
	block, light, cog = getTextures(entity)
	lightSprite = Ahorn.getSprite(light, "Gameplay")
	tilesWidth = div(width, 8)
	tilesHeight = div(height, 8)
	nodes = deepcopy(entity.data["nodes"])
	pushfirst!(nodes, Ahorn.position(entity))
	stopPoint = length(nodes) - 1
	for node in 1:stopPoint
		x, y = Int.(nodes[node])
		nx, ny = Int.(nodes[node + 1])

		cx, cy = x + width / 2, y + height / 2
		cnx, cny = nx + width / 2, ny + height / 2

		length = sqrt((x - nx)^2 + (y - ny)^2)
		theta = atan(cny - cy, cnx - cx)

		Ahorn.Cairo.save(ctx)

		Ahorn.translate(ctx, cx, cy)
		Ahorn.rotate(ctx, theta)

		Ahorn.setSourceColor(ctx, ropeColor)
		Ahorn.set_antialias(ctx, 1)
		Ahorn.set_line_width(ctx, 1);

		# Offset for rounding errors
		Ahorn.move_to(ctx, 0, 4 + (theta <= 0))
		Ahorn.line_to(ctx, length, 4 + (theta <= 0))

		Ahorn.move_to(ctx, 0, -4 - (theta > 0))
		Ahorn.line_to(ctx, length, -4 - (theta > 0))

		Ahorn.stroke(ctx)
		
		Ahorn.Cairo.restore(ctx)
		
		Ahorn.drawSprite(ctx, cog, cnx, cny)
	end
	
	x, y = Ahorn.position(entity)
	
	Ahorn.drawRectangle(ctx, x + 2, y + 2, width - 4, height - 4, (0.0, 0.0, 0.0, 1.0))

	for i in 2:tilesWidth - 1
		Ahorn.drawImage(ctx, block, x + (i - 1) * 8, y, 8, 0, 8, 8)
		Ahorn.drawImage(ctx, block, x + (i - 1) * 8, y + height - 8, 8, 16, 8, 8)
	end

	for i in 2:tilesHeight - 1
		Ahorn.drawImage(ctx, block, x, y + (i - 1) * 8, 0, 8, 8, 8)
		Ahorn.drawImage(ctx, block, x + width - 8, y + (i - 1) * 8, 16, 8, 8, 8)
	end

	Ahorn.drawImage(ctx, block, x, y, 0, 0, 8, 8)
	Ahorn.drawImage(ctx, block, x + width - 8, y, 16, 0, 8, 8)
	Ahorn.drawImage(ctx, block, x, y + height - 8, 0, 16, 8, 8)
	Ahorn.drawImage(ctx, block, x + width - 8, y + height - 8, 16, 16, 8, 8)

	Ahorn.drawImage(ctx, lightSprite, x + floor(Int, (width - lightSprite.width) / 2), y)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::ChainMover, room::Maple.Room)
    renderChainMover(ctx, entity)
end

end