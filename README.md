# Commands:

<table>
   <td>Command</td>
   <td>Permission</td>
   <td>Description</td>
   <tr>
    <td>/rdoor</td>
    <td>Picklock.doorblacklist</td>
    <td>You can add/remove storages and doors from the blacklist for picklocking</td>
   </tr>
   <tr>
    <td></td>
    <td>Picklock</td>
    <td>Allows you to picklock into storages and doors!</td>
   </tr>
</table>

# Configuration:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Config xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <PickLock_Config>
    <PickLock>
      <Webhook>Webhook Here!</Webhook>
      <PickLockBreak>50</PickLockBreak>
      <Rules>
        <PickLockInfo>
          <ID>281</ID>
          <PickLockID>1353</PickLockID>
          <Time>10</Time>
          <Chance>25</Chance>
        </PickLockInfo>
        <PickLockInfo>
          <ID>282</ID>
          <PickLockID>1353</PickLockID>
          <Time>10</Time>
          <Chance>25</Chance>
        </PickLockInfo>
        <PickLockInfo>
          <ID>328</ID>
          <PickLockID>1353</PickLockID>
          <Time>10</Time>
          <Chance>25</Chance>
        </PickLockInfo>
      </Rules>
    </PickLock>
  </PickLock_Config>
  <LogoImage>https://i.imgur.com/lv4E8TR.jpg</LogoImage>
  <AutoCloseDoors>false</AutoCloseDoors>
  <AutoCloseDoorsDelay>2500</AutoCloseDoorsDelay>
</Config>
```
