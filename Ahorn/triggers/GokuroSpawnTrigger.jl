module Sardine7GokuroSpawnTrigger

using ..Ahorn, Maple

@mapdef Trigger "Sardine7/GokuroSpawnTrigger" GokuroSpawnTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, chaseWaitTimes::String="1.0,2.0,3.0,2.0,3.0", bouncy::Bool=true, yApproachDuringRespawn::Bool=false, yApproachSpeed::Real=100.0, dieFast::Bool=false, attackMaxSpeed::Real=500.0, chronosBarrier::Real=200.0, luigi::Bool=false, extraApproach::Bool=true, yPosition::Real=-1.0, volume::Real=1.0, attackCount::Integer=-1)

const placements = Ahorn.PlacementDict(
    "Gokuro (Spawn, Sardine7)" => Ahorn.EntityPlacement(
        GokuroSpawnTrigger,
        "rectangle"
    )
)

end