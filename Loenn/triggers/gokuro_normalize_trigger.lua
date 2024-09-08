local gokuroNormalize = {}

gokuroNormalize.name = "Sardine7/GokuroNormalizeTrigger"

gokuroNormalize.fieldInformation = {
    count = {
        fieldType = "integer",
        minimumValue = 1
    }
}

gokuroNormalize.placements = {
    name = "gokuro_normalize",
    data = {
        count = 1
    }
}

return gokuroNormalize
