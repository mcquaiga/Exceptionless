﻿using System;
using System.Threading.Tasks;
using Exceptionless.Core.Component;
using Exceptionless.Core.Extensions;
using Exceptionless.Core.Pipeline;
using Exceptionless.Core.Models;

namespace Exceptionless.Core.Plugins.EventProcessor {
    [Priority(10)]
    public class NotFoundPlugin : EventProcessorPluginBase {
        public override Task EventProcessingAsync(EventContext context) {
            if (context.Event.Type != Event.KnownTypes.NotFound)
                return TaskHelper.Completed();

            context.Event.Data.Remove(Event.KnownDataKeys.EnvironmentInfo);
            context.Event.Data.Remove(Event.KnownDataKeys.TraceLog);

            var req = context.Event.GetRequestInfo();
            if (req == null)
                return TaskHelper.Completed();

            if (String.IsNullOrWhiteSpace(context.Event.Source)) {
                context.Event.Message = null;
                context.Event.Source = req.GetFullPath(includeHttpMethod: true, includeHost: false, includeQueryString: false);
            }

            context.Event.Data.Remove(Event.KnownDataKeys.Error);
            context.Event.Data.Remove(Event.KnownDataKeys.SimpleError);

            return TaskHelper.Completed();
        }
    }
}