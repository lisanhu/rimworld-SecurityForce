# CHANGE LOG

## 06-12-2025 EST
- cp contents from 1.5 to 1.6
- This change log file is first created for 1.6 versions
- 1.6.4488-beta
- Update About.xml to add 1.6 tag
- update .gitignore to ignore the obj files
- update 1.6 .csproj file to use 1.6 beta interface
- CygnusStandardTights.cs update tick from public to protected
  - Since this is to heal the Wearer, which is important
- PolarisShieldBelt_IV.cs update tick to protected
  - Since this shield is critical that needs to run every tick
- update roo configs
- QuestScriptDef.CanRun is updated with 2 parameters
  - It needs a target as 2nd parameter (map, world,  etc.)
  - CompUseEffect_GenQuestInPoints.cs
- bulletChanceToStartFire field is removed from ProjectileProperties class
  - Corresponding lines in CaniculaBullet.cs has been removed
  - The new ProjectileProperties will need to specify a thing def or using other methods to reproduce the fire effect
  - Since this fire chance is not defined in the xml, it might be 0 already, so we just commented out these lines and see
- PawnKindDef.defaultFactionType field has been renamed to defaultFactionDef (but it's xml aliased)
  - BodyFixTrigger.cs Notify_Equipped function update the field name