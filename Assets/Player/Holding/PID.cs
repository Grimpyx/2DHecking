
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PID
{
    public enum DerivativeMeasurement
    {
        Velocity,
        ErrorRateOfChange
    }

    //PID coefficients
    public float proportionalGain;
    public float integralGain;
    public float derivativeGain;

    public float outputMin = -1;
    public float outputMax = 1;
    public float integralSaturation;
    public DerivativeMeasurement derivativeMeasurement;
    public float ValueLast { get; private set; }
    public float ErrorLast { get; private set; }
    public float IntegrationStored { get; private set; }
    //public float velocity;
    public bool DerivativeInitialized { get; private set; }

    public PID(float proportionalGain, float integralGain, float derivativeGain, float outputMin, float outputMax, float integralSaturation, DerivativeMeasurement derivativeMeasurement)
    {
        this.proportionalGain = proportionalGain;
        this.integralGain = integralGain;
        this.derivativeGain = derivativeGain;
        this.outputMin = outputMin;
        this.outputMax = outputMax;
        this.integralSaturation = integralSaturation;
        this.derivativeMeasurement = derivativeMeasurement;
    }

    public static PID GetDefaultPID()
    {
        return new PID(1, 0, 1, -999999, 999999, 0, DerivativeMeasurement.Velocity);
    }

    public void Reset()
    {
        DerivativeInitialized = false;
    }

    public float Update(float dt, float currentValue, float targetValue)
    {
        if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));

        float error = targetValue - currentValue;

        //calculate P term
        float P = proportionalGain * error;

        //calculate I term
        IntegrationStored = Mathf.Clamp(IntegrationStored + (error * dt), -integralSaturation, integralSaturation);
        float I = integralGain * IntegrationStored;

        //calculate both D terms
        float errorRateOfChange = (error - ErrorLast) / dt;
        ErrorLast = error;

        float valueRateOfChange = (currentValue - ValueLast) / dt;
        ValueLast = currentValue;
        //velocity = valueRateOfChange;

        //choose D term to use
        float deriveMeasure = 0;

        if (DerivativeInitialized)
        {
            if (derivativeMeasurement == DerivativeMeasurement.Velocity)
            {
                deriveMeasure = -valueRateOfChange;
            }
            else
            {
                deriveMeasure = errorRateOfChange;
            }
        }
        else
        {
            DerivativeInitialized = true;
        }

        float D = derivativeGain * deriveMeasure;

        float result = P + I + D;

        return Mathf.Clamp(result, outputMin, outputMax);
    }

    float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;   //calculate modular difference, and remap to [-180, 180]
    }

    public float UpdateAngle(float dt, float currentAngle, float targetAngle)
    {
        if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));
        float error = AngleDifference(targetAngle, currentAngle);
        ErrorLast = error;

        //calculate P term
        float P = proportionalGain * error;

        //calculate I term
        IntegrationStored = Mathf.Clamp(IntegrationStored + (error * dt), -integralSaturation, integralSaturation);
        float I = integralGain * IntegrationStored;

        //calculate both D terms
        float errorRateOfChange = AngleDifference(error, ErrorLast) / dt;
        ErrorLast = error;

        float valueRateOfChange = AngleDifference(currentAngle, ValueLast) / dt;
        ValueLast = currentAngle;
        //velocity = valueRateOfChange;

        //choose D term to use
        float deriveMeasure = 0;

        if (DerivativeInitialized)
        {
            if (derivativeMeasurement == DerivativeMeasurement.Velocity)
            {
                deriveMeasure = -valueRateOfChange;
            }
            else
            {
                deriveMeasure = errorRateOfChange;
            }
        }
        else
        {
            DerivativeInitialized = true;
        }

        float D = derivativeGain * deriveMeasure;

        float result = P + I + D;

        return Mathf.Clamp(result, outputMin, outputMax);
    }
}

/*Footer
© 2022 GitHub, Inc.
Footer navigation
Terms
Privacy
Security
Status
Docs
Contact GitHub
Pricing
API
Training
Blog*/