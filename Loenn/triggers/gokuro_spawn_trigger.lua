local gokuroSpawn = {}

gokuroSpawn.name = "Sardine7/GokuroSpawnTrigger"

gokuroSpawn.fieldInformation = {
    yApproachSpeed = {
        fieldType = "number",
        minimumValue = 0.0
    },
    attackMaxSpeed = {
        fieldType = "number",
        minimumValue = 0.0
    },
    chronosBarrier = {
        fieldType = "number",
        minimumValue = 0.0
    },
    yPosition = {
        fieldType = "number",
        minimumValue = -1.0
    },
    volume = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    },
    attackCount = {
        fieldType = "integer",
        minimumValue = -1
    }
}

gokuroSpawn.placements = {
    name = "gokuro_spawn",
    data = {
        chaseWaitTimes = "1.0,2.0,3.0,2.0,3.0",
        bouncy = true,
        yApproachDuringRespawn = false,
        yApproachSpeed = 100.0,
        dieFast = false,
        attackMaxSpeed = 500.0,
        chronosBarrier = 200.0,
        luigi = false,
        extraApproach = true,
        yPosition = -1.0,
        volume = 1.0,
        attackCount = -1
    }
}

return gokuroSpawn
