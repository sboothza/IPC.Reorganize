# Interprocess.Transmogrify.Json

Snap-in (mostly) replacement for System.Text.Json and Newtonsoft.Json

Contains a lot of extra features that are not handled consistently in both:
 - Null value handling
 - Automatic Naming (Pascal, camel and snake)
 - Case-sensitivity
 - Naming overrides
 - Error handling
 - Name remapping

## Compatibility

Built with .NET 8

## Performance

Slightly faster than both System.Text.Json and Newtonsoft.Json

## History

Originally a fork from [Stephen.JsonSerializerLib](https://github.com/sboothza/JsonSerializer)  
Been cleaned up and hardened