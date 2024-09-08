local humbleLookout = {}

humbleLookout.name = "Sardine7/HumbleLookout"
humbleLookout.depth = -8500
humbleLookout.justification = {0.5, 1.0}
humbleLookout.nodeLineRenderType = "line"
humbleLookout.texture = "objects/lookout/lookout05"
humbleLookout.nodeLimits = {0, -1}
humbleLookout.placements = {
    name = "watchtower",
    alternativeName = {"lookout", "binoculars"},
    data = {
        summit = false,
        onlyY = false,
        attractive = true,
        talkative = true
    }
}

return humbleLookout
