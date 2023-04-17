using Elements;
using Elements.Geometry;
using System.Collections.Generic;

namespace MJTelescoperPlan
{
      public static class MJTelescoperPlan
    {
        /// <summary>
        /// The MJTelescoperPlan function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A MJTelescoperPlanOutputs instance containing computed results and the model with any new elements.</returns>
        public static MJTelescoperPlanOutputs Execute(Dictionary<string, Model> inputModels, MJTelescoperPlanInputs input)
        {
          var outputs = new MJTelescoperPlanOutputs();
          Console.WriteLine("polygon: " + input.BuildingPolygon);
          if(input.BuildingPolygon == null) return outputs;

          var plan = new TelescopePlan(
           new Profile(input.BuildingPolygon),
           input.TelescopeSpread, 
           input.RecurseLimit,
           input.BaseHeight,
           input.MaxHeight,
           input.TelescopeExponent,
           input.TelescopeStepPercent);

          outputs.Model.AddElements(plan);
          return outputs;
        }
      }
}