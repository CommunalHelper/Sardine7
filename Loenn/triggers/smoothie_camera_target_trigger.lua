local enums = require("consts.celeste_enums")

local smoothieCameraTarget = {}

smoothieCameraTarget.name = "Sardine7/SmoothieCameraTargetTrigger"
smoothieCameraTarget.nodeLimits = {1, 1}
smoothieCameraTarget.fieldInformation = {
    positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}
smoothieCameraTarget.placements = {
    name = "smoothie_camera_target",
    data = {
        xLerpStrength = 0.0,
        yLerpStrength = 0.0,
        positionMode = "NoEffect",
        xOnly = false,
        yOnly = false,
        deleteFlag = ""
    }
}

return smoothieCameraTarget
