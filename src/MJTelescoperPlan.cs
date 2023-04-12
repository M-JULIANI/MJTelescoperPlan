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

          var plan = new TelescopePlan(new Profile(input.BuildingPolygon), input.MaxTelescoping, input.RecurseLimit);

          // var plans = input.Overrides.TelescopePlan.CreateElements(
          //   input.Overrides.Additions.TelescopePlan,
          //   input.Overrides.Removals.TelescopePlan,
          //   (add)=> new TelescopePlan(add, ComputeDistance, input.MaxTelescoping),
          //   (plan, identity)=> plan.Match(identity),
          //   (plan, edit)=> plan.Update(edit)
          // );

         // var mass = new Mass(new Profile(input.BuildingPolygon));
        //  if(mass!=null) outputs.Model.AddElement(mass);

          outputs.Model.AddElements(plan);
          return outputs;
        }
      }
}