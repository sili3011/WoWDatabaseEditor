using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WDE.Common.Database;
using WDE.Common.Parameters;
using WDE.DatabaseEditors.Models;

namespace WDE.DatabaseEditors.Parameters;

public class DbScriptRandomTemplateTargetValueParameter : IAsyncContextualParameter<long, DatabaseEntity>, ICustomPickerContextualParameter<long>
{
    private readonly IParameterPickerService pickerService;
    private readonly IAsyncParameter<long>? broadcastTextsParameter;

    public DbScriptRandomTemplateTargetValueParameter(IParameterPickerService pickerService,
        IParameter<long> broadcastTextsParameter)
    {
        this.pickerService = pickerService;
        if (broadcastTextsParameter is IAsyncParameter<long> asyncBroadcast)
            this.broadcastTextsParameter = asyncBroadcast;
    }

    public Task<(long, bool)> PickValue(long value, object context)
    {
        IParameter<long> parameter = Parameter.Instance;
        if (context is DatabaseEntity entity)
        {
            var cell = entity.GetTypedValueOrThrow<long>("type");
            if (cell == (int)IMangosDatabaseProvider.RandomTemplateType.Text && broadcastTextsParameter != null)
            {
                parameter = broadcastTextsParameter;
            }
            else if (cell == (int)IMangosDatabaseProvider.RandomTemplateType.RelayScript)
            {
                // maybe in the future we might have something better here
                parameter = Parameter.Instance;
            }
        }

        return pickerService.PickParameter(parameter, value);
    }
    
    public string ToString(long value, DatabaseEntity context)
    {
        return value.ToString();
    }

    public Task<string> ToStringAsync(long value, CancellationToken token, DatabaseEntity entity)
    {
        var cell = entity.GetTypedValueOrThrow<long>("type");
        if (cell == (int)IMangosDatabaseProvider.RandomTemplateType.Text && broadcastTextsParameter != null)
            return broadcastTextsParameter.ToStringAsync(value, token);
        return Task.FromResult(value.ToString());
    }

    public string? Prefix => null;
    public bool HasItems => true;
    public bool AllowUnknownItems => true;
    public string ToString(long value) => value.ToString();
    public Dictionary<long, SelectOption>? Items => null;
}