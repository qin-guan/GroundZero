using MathNet.Numerics;

namespace GroundZero.Gavel;

/// <summary>
/// CrowdBT port to C# based on HackMIT's Gavel
/// https://github.com/anishathalye/gavel/blob/master/gavel/crowd_bt.py
/// </summary>
public class CrowdBt
{
    public const double Gamma = 0.1;
    public const double Lambda = 1.0;
    public const double Kappa = 0.0001;
    public const double MuPrior = 0.0;
    public const double SigmaSqPrior = 1.0;
    public const double AlphaPrior = 10.0;
    public const double BetaPrior = 1.0;
    public const double Epsilon = 0.25;

    public static T Argmax<T>(Func<T, double> f, IEnumerable<T> xs)
    {
        return xs.Aggregate((agg, next) => f(next) > f(agg) ? next : agg);
    }

    public static double DivergenceGaussian(double mu1, double sigmaSq1, double mu2, double sigmaSq2)
    {
        var ratio = sigmaSq1 / sigmaSq2;
        return Math.Pow(mu1 - mu2, 2) / (2 * sigmaSq2) + (ratio - 1 - Math.Log(ratio)) / 2;
    }

    public static double DivergenceBeta(double alpha1, double beta1, double alpha2, double beta2)
    {
        return SpecialFunctions.BetaLn(alpha2, beta2) - SpecialFunctions.BetaLn(alpha1, beta1) +
               (alpha1 - alpha2) * SpecialFunctions.DiGamma(alpha1) +
               (beta1 - beta2) * SpecialFunctions.DiGamma(beta1) +
               (alpha2 - alpha1 + beta2 - beta1) * SpecialFunctions.DiGamma(alpha1 + beta1);
    }

    public static (double, double, double, double, double, double) Update(
        double alpha,
        double beta,
        double muWinner,
        double sigmaSqWinner,
        double muLoser,
        double sigmaSqLoser
    )
    {
        var (updatedAlpha, updatedBeta, _) =
            UpdatedAnnotator(alpha, beta, muWinner, sigmaSqWinner, muLoser, sigmaSqLoser);
        var (updatedMuWinner, updatedMuLoser) =
            UpdatedMus(alpha, beta, muWinner, sigmaSqWinner, muLoser, sigmaSqLoser);
        var (updatedSigmaSqWinner, updatedSigmaSqLoser) =
            UpdatedSigmaSqs(alpha, beta, muWinner, sigmaSqWinner, muLoser, sigmaSqLoser);

        return (updatedAlpha, updatedBeta, updatedMuWinner, updatedSigmaSqWinner, updatedMuLoser, updatedSigmaSqLoser);
    }

    public static double ExpectedInformationGain(
        double alpha,
        double beta,
        double muA,
        double sigmaSqA,
        double muB,
        double sigmaSqB
    )
    {
        var (alpha1, beta1, c) = UpdatedAnnotator(alpha, beta, muA, sigmaSqA, muB, sigmaSqB);
        var (muA1, muB1) = UpdatedMus(alpha, beta, muA, sigmaSqA, muB, sigmaSqB);
        var (sigmaSqA1, sigmaSqB1) = UpdatedSigmaSqs(alpha, beta, muA, sigmaSqA, muB, sigmaSqB);
        var (alpha2, beta2, _) = UpdatedAnnotator(alpha, beta, muB, sigmaSqB, muA, sigmaSqA);
        var valueTuple = UpdatedMus(alpha, beta, muB, sigmaSqB, muA, sigmaSqA);
        var updatedSigmaSqs = UpdatedSigmaSqs(alpha, beta, muB, sigmaSqB, muA, sigmaSqA);

        return c *
               (
                   DivergenceGaussian(muA1, sigmaSqA1, muA, sigmaSqA) +
                   DivergenceGaussian(muB1, sigmaSqB1, muB, sigmaSqB) +
                   Gamma * DivergenceBeta(alpha1, beta1, alpha, beta)
               )
               + (1 - c) *
               (
                   DivergenceGaussian(valueTuple.Item2, updatedSigmaSqs.Item2, muA, sigmaSqA) +
                   DivergenceGaussian(valueTuple.Item1, updatedSigmaSqs.Item1, muB, sigmaSqB) +
                   Gamma * DivergenceBeta(alpha2, beta2, alpha, beta)
               );
    }

    private static (double, double) UpdatedMus(
        double alpha,
        double beta,
        double muWinner,
        double sigmaSqWinner,
        double muLoser,
        double sigmaSqLoser
    )
    {
        var mult = alpha * double.Exp(muWinner) / (alpha * double.Exp(muWinner) + beta * double.Exp(muLoser)) -
                   double.Exp(muWinner) / (double.Exp(muWinner) + double.Exp(muLoser));
        var updatedMuWinner = muWinner + sigmaSqWinner * mult;
        var updatedMuLoser = muLoser - sigmaSqLoser * mult;
        return (updatedMuWinner, updatedMuLoser);
    }

    private static (double, double) UpdatedSigmaSqs(
        double alpha,
        double beta,
        double muWinner,
        double sigmaSqWinner,
        double muLoser,
        double sigmaSqLoser
    )
    {
        var mult = alpha * Math.Exp(muWinner) * beta * Math.Exp(muLoser) /
                   Math.Pow(alpha * Math.Exp(muWinner) + beta * Math.Exp(muLoser), 2) -
                   Math.Exp(muWinner) * Math.Exp(muLoser) / Math.Pow(Math.Exp(muWinner) + Math.Exp(muLoser), 2);

        var updatedSigmaSqWinner = sigmaSqWinner * Math.Max(1 + sigmaSqWinner * mult, Kappa);
        var updatedSigmaSqLoser = sigmaSqLoser * Math.Max(1 + sigmaSqLoser * mult, Kappa);

        return (updatedSigmaSqWinner, updatedSigmaSqLoser);
    }

    private static (double, double, double) UpdatedAnnotator(
        double alpha,
        double beta,
        double muWinner,
        double sigmaSqWinner,
        double muLoser,
        double sigmaSqLoser
    )
    {
        var c1 = double.Exp(muWinner) / (double.Exp(muWinner) + double.Exp(muLoser)) + 0.5 *
            (sigmaSqWinner + sigmaSqLoser) *
            (double.Exp(muWinner) * double.Exp(muLoser) * (double.Exp(muLoser) - double.Exp(muWinner))) /
            Math.Pow(double.Exp(muWinner) + double.Exp(muLoser), 3);
        var c2 = 1 - c1;
        var c = (c1 * alpha + c2 * beta) / (alpha + beta);

        var expt = (c1 * (alpha + 1) * alpha + c2 * alpha * beta) / (c * (alpha + beta + 1) * (alpha + beta));
        var exptSq = (c1 * (alpha + 2) * (alpha + 1) * alpha + c2 * (alpha + 1) * alpha * beta) /
                     (c * (alpha + beta + 2) * (alpha + beta + 1) * (alpha + beta));

        var variance = exptSq - Math.Pow(expt, 2);
        var updatedAlpha = (expt - exptSq) * expt / variance;
        var updatedBeta = (expt - exptSq) * (1 - expt) / variance;

        return (updatedAlpha, updatedBeta, c);
    }
}
