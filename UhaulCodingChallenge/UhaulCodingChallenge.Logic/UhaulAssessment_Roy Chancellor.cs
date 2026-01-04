using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaulCodingChallenge.Logic
{
    /*
	This assignment takes most candidates 10-20 minutes.
	Please include all code in one file and add your name to the filename.
	Your returned file should contain several classes.
    */

    /*
        Your goal in this assignment will be to demonstrate your ability to create
        a MAINTAINABLE, CONCISE, and EXTENSIBLE solution to the problem below that also meets
        all the requirements explained.  There are several solutions to this problem; we are not looking for a specific one.
        It is up to you to choose one that fits the criteria and best meets the three atrributes above.
    */

    /* 
        First, create the following four kinds of U-Haul equipment.
        Do not worry about adding more detail than necessary on each kind of equipment:

        Moving Truck:
            * U-Haul's standard vehicle - which can both carry cargo and tow other equipment.
            * Has a Max Cargo Weight (an integer which may vary for each instance of the Moving Truck).
            * Has a Tow Rating (an integer which may vary for each instance of the Moving Truck).
            * Always has 4 wheels.
            * Here is an example of a Moving Truck which is both loaded with cargo and towing equipment:
            * https://cdnep-uhaul-uhaulcom-global-p-001.azureedge.net/uhaulcom/cdn/Misc/Trailers/trailer-transfer.jpg
        Trailer:
            * A piece of equipment that carries cargo and can be towed behind a U-Haul or customer vehicle.
            * Has a Max Cargo Weight (an integer which may vary for each instance of the Trailer).
            * Has a variable number of wheels.
            * Since Trailers can't tow other equipment, it makes no sense for the Trailer to define a Tow Rating.
            * Here is an example of one of U-Haul's 2-Wheel Trailers being towed:
            * https://www.uhaul.com/Blog/wp-content/uploads/2016/11/header.png
        Tow Dolly:
            * A piece of equipment that can't carry cargo, but can tow a customer's vehicle behind another vehicle.
            * Has a Tow Rating (an integer which may vary for each instance of the Tow Dolly).
            * Since Tow Dollies can't carry cargo, it makes no sense for the Tow Dolly to define a Max Cargo Weight.
            * Always has 2 or 4 wheels.
            * Here is an example of a 4-wheel Tow Dolly that is towing a customer's auto:
            * https://www.uhaul.com/Blog/wp-content/uploads/2013/03/Hit-the-Road.jpg
        Electric Cart:
            * A vehicle used to assist customers around large storage lots.  It does not carry cargo or tow other equipment.
            * Always has 4 wheels.
            * Since Electric Carts don't carry cargo or tow,
              it makes no sense for the Electric Cart to define a Max Cargo Weight or Tow Rating.
            * Here is a U-Haul Electric Cart in action:
            * https://www.uhaul.com/Locations/GetPhoto.ashx?id=1055990&size=5
    */

    /*
        Last, you will populate the below static UHaulEquipmentValidation class
        by adding the following three STATIC methods (and ONLY these three methods):

        Method #1:	GetTowRating -	Returns the equipment's Tow Rating
        Method #2:	IsHeavyDuty	-	Returns true if the equipment's Max Cargo Weight is over 1500.  False otherwise.
        Method #3:	GetTollFees -	Returns the cost of tolls for this equipment which is calculated in this way:
                                        # number of axles on the equipment multiplied by an input toll amount (decimal)
                                    You can assume the equipment has an axle for every 2 wheels.
    */

    /*
        Each of the three methods you create may take one or more parameters.
        You should not remove the static keyword from the class.

        GetTollFees should be allowed to be called on ANY U-Haul equipment
        However the other two methods should only be possible for equipment that makes sense for that action.
        For example, it doesn't make sense to be able to call GetTowRating with Trailers or Electric Carts.

        Do not provide code executing or testing your code (please no static mains).

        If you are significantly more comfortable with a different object-oriented language,
        you may change the file type and static class to its equivalent in your preferred language.
        All above requirements and metrics must be honored, regardless of language choice.
    */

    /***************************************************************************************************************/
    /*
        Thought process:

        Moving Truck        Trailer             Tow Dolly           Electric Cart
        Max Cargo Weight    Max Cargo Weight    ---------           --------
        Tow Rating          -------             ---------           --------
        Always 4 wheels     Variable wheels     Variable wheels     Always 4 wheels

        All vehicle types have wheels, so define an interface for a vehicle that has a wheels property
        Two of the vehicles can carry cargo, so define an interface for a cargo vehicle with a cargo weight property
        One of the vehicles can tow, so define an interface for a tow vehicle with a tow rating property.

        From these interfaces, create concrete classes for the four vehicle types.
    */
    /***************************************************************************************************************/

    public interface IVehicle
    {
        // Require only a getter. The concrete implementation can decide whether to have a setter or not.
        int NumberOfWheels { get; }
    }
    public interface ICargoVehicle
    {
        int MaxCargoWeight { get; set; }
    }
    public interface ITowVehicle
    {
        int TowRating { get; set; }
    }

    public class MovingTruck : IVehicle, ICargoVehicle, ITowVehicle
    {
        public int NumberOfWheels { get => 4; }
        public int TowRating { get; set; }
        public int MaxCargoWeight { get; set; }
    }
    
    public class Trailer : IVehicle, ICargoVehicle
    {
        public int NumberOfWheels { get; set; }
        public int MaxCargoWeight { get; set; }
    }
    
    public class TowDolly : IVehicle
    {
        public int NumberOfWheels { get; set; }
    }
    
    public class ElectricCart : IVehicle
    {
        public int NumberOfWheels { get => 4; }
    }

    internal static class UHaulEquipmentValidation
    {
        private const int WHEELS_PER_AXLE = 2;

        // Each of the method receives a type that is appropriate for the method.
        // Trying to pass an object of the wrong type will cause a compile-time error,
        // thus making the method callable only by the correct object type.
        
        // Callable only for equipment that has a tow rating
        public static int GetTowRating(ITowVehicle towVehicle)
        {
            if (towVehicle == null)
            {
                throw new Exception("Passed-in tow vehicle parameter is null");
            }
            return towVehicle.TowRating;
        }
        
        // Callable only for equipment that has a cargo rating
        public static bool IsHeavyDuty(ICargoVehicle cargoVehicle, int weightThreshold = 1500)
        {
            if (cargoVehicle == null)
            {
                throw new Exception("Passed-in cargo vehicle parameter is null");
            }
            return cargoVehicle.MaxCargoWeight > weightThreshold;
        }
        
        // Callable for ANY U-Haul equipment
        public static decimal? GetTollFees(IVehicle vehicle, decimal tollFee)
        {
            if (vehicle == null || vehicle.NumberOfWheels < 2 || tollFee < 0)
            {
                throw new Exception($"Passed-in parameters are invalid | vehicle: {vehicle} | NumberOfWheels: {vehicle?.NumberOfWheels} | toll fee: {tollFee}");
            }
            return vehicle.NumberOfWheels * tollFee / WHEELS_PER_AXLE;
        }
    }
}
