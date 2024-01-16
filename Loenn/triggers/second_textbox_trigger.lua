local enums = require("consts.celeste_enums")

local secondTextbox = {}

secondTextbox.name = "Sardine7/SecondTextboxTrigger"
secondTextbox.fieldInformation = {
    death_count = {
        fieldType = "integer",
    },
    lifespan = {
        fieldType = "number",
        minimumValue = 0.0
    },
    mode = {
        options = enums.mini_textbox_trigger_modes,
        editable = false
    }
}
secondTextbox.placements = {
    name = "second_textbox",
    data = {
        dialog_id = "",
        mode = "OnPlayerEnter",
        only_once = true,
        death_count = -1,
        lifespan = "3.0"
    }
}

return secondTextbox
