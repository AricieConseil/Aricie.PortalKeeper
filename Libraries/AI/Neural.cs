using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Security;
using AIFH_Vol3.Core.Error;
using AIFH_Vol3.Core.General.Data;
using AIFH_Vol3.Core.Learning;
using AIFH_Vol3.Examples.Learning;
using AIFH_Vol3_Core.Core.ANN;
using AIFH_Vol3_Core.Core.ANN.Activation;
using AIFH_Vol3_Core.Core.ANN.Train;
using Aricie.ComponentModel;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.Entities;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Files;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.PortalKeeper.AI.CSP;
using java.lang;
using Microsoft.MSR.CNTK.Extensibility.Managed;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Object = System.Object;

namespace Aricie.PortalKeeper.AI.Learning.Neural
{
    public enum NeuralLayerType
    {
        BasicLayer,
        Conv2D,
        MaxPool,
        Dropout,
    }

    public enum ActivationFunctionType
    {
        Linear,
        Sigmoid,
        TanH,
        SoftMax,
        ReLU
    }

    public class NeuralLayerInfo
    {

        public NeuralLayerInfo()
        {
            NeuronsPerDimension = new OneOrMore<int>(10);
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof (StringEnumConverter))]
        public NeuralLayerType LayerType { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof (StringEnumConverter))]
        public ActivationFunctionType ActivationFunction { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.BasicLayer, NeuralLayerType.Dropout)]
        public bool HasBias { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.BasicLayer, NeuralLayerType.Dropout)]
        public OneOrMore<int> NeuronsPerDimension { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.Conv2D)]
        public int ConvNbFilters { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.Conv2D)]
        public int ConvNbRows { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.Conv2D)]
        public int ConvNbColumns { get; set; }

        [ConditionalVisible("LayerType", false, true, NeuralLayerType.Dropout)]
        public double DropOut { get; set; }

        public IActivationFunction GetActivationFunction()
        {
            IActivationFunction activation;
            switch (ActivationFunction)
            {
                case ActivationFunctionType.Linear:
                    activation = new ActivationLinear();
                    break;
                case ActivationFunctionType.Sigmoid:
                    activation = new ActivationSigmoid();
                    break;
                case ActivationFunctionType.TanH:
                    activation = new ActivationTANH();
                    break;
                case ActivationFunctionType.SoftMax:
                    activation = new ActivationSoftMax();
                    break;
                case ActivationFunctionType.ReLU:
                    activation = new ActivationReLU();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return activation;
        }


        public ILayer GetLayer()
        {
            var activation = GetActivationFunction();
            ILayer toReturn;
            switch (LayerType)
            {
                case NeuralLayerType.BasicLayer:
                    toReturn = new BasicLayer(activation, HasBias, NeuronsPerDimension.All.ToArray());
                    break;
                case NeuralLayerType.Conv2D:
                    toReturn = new Conv2DLayer(activation, ConvNbFilters, ConvNbRows, ConvNbColumns);
                    break;
                case NeuralLayerType.Dropout:
                    toReturn = new DropoutLayer(activation, HasBias, NeuronsPerDimension.One, DropOut);
                    break;
                case NeuralLayerType.MaxPool:
                    toReturn = new MaxPoolLayer(NeuronsPerDimension.All.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return toReturn;
        }

    }


    public interface INeuralNetwork
    {

        Object GetNetwork();

    }


    public class BasicNetworkInfo : INeuralNetwork
    {
        public BasicNetworkInfo()
        {
            Layers = new List<NeuralLayerInfo>();
        }

        public List<NeuralLayerInfo> Layers { get; set; }

        public BasicNetwork GetNetwork()
        {
            var network = new BasicNetwork();
            foreach (NeuralLayerInfo layer in Layers)
            {
                var objLayer = layer.GetLayer();
                network.AddLayer(objLayer);
            }
            network.FinalizeStructure();
            network.Reset();
            return network;
        }

        object INeuralNetwork.GetNetwork()
        {
            return GetNetwork();
        }
    }

    public enum LearningMethod
    {
        BackPropagation,
        ResilientPropagation,
        TrainAnneal,
        SupervisedDeepBelief
    }


    public interface ILearningMethodInfo
    {
        ILearningMethod GetLearningMethod(object owner, IContextLookup globalVars, Object theNetwork, IEnumerable data);
    }

    public class BackPropagationInfo : ILearningMethodInfo
    {

        public BackPropagationInfo()
        {
            LearningRate = new SimpleOrExpression<double>(1e-4);
            Momentum = new SimpleOrExpression<double>(0.9);
            L1 = new SimpleOrExpression<double>(0);
            L2 = new SimpleOrExpression<double>(1e-11);
        }

        public SimpleOrExpression<double> LearningRate { get; set; }

        public SimpleOrExpression<double> Momentum { get; set; }

        /// <summary>
        ///     L1 regularization weighting, 0.0 for none.
        /// </summary>
        public SimpleOrExpression<double> L1 { get; set; }

        /// <summary>
        ///     L2 regularization weighting, 0.0 for none.
        /// </summary>
        public SimpleOrExpression<double> L2 { get; set; }


        public BackPropagation GetLearningMethod(object owner, IContextLookup globalVars, BasicNetwork theNetwork,
            IList<BasicData> data)
        {
            double theLearningRate = LearningRate.GetValue(owner, globalVars);
            double theMomentum = Momentum.GetValue(owner, globalVars);
            var theL1 = L1.GetValue(owner, globalVars);
            var theL2 = L2.GetValue(owner, globalVars);
            var toReturn = new BackPropagation(theNetwork, data, theLearningRate, theMomentum);
            toReturn.L1 = theL1;
            toReturn.L2 = theL2;
            return toReturn;
        }

        public ILearningMethod GetLearningMethod(object owner, IContextLookup globalVars, object theNetwork,
            IEnumerable data)
        {
            return GetLearningMethod(owner, globalVars, (BasicNetwork) theNetwork, (IList<BasicData>) data);
        }

    }

    //public enum NetworkTrainMode
    //{
    //    PerformIterations,
    //    RegressionValidateEarlyStop,
    //    ClassifyValidateEarlyStop
    //}


    public interface ITrainingInfo
    {
        void PerformTraining(ILearningMethod method, object model, IEnumerable validationData);
    }


    public class PerformIterationsInfo : ITrainingInfo
    {
        public int MaxIterations { get; set; }

        public double TargetScore { get; set; }

        public bool ShouldMinimize { get; set; }

        public void PerformTraining(ILearningMethod method, object model, IEnumerable validationData)
        {
            new SimpleLearn().PerformIterations(method, MaxIterations, TargetScore, ShouldMinimize);
        }
    }

    public enum ErrorCaclulationType
    {
        MeanSquare,
        RootMeanSquare,
        SumOfSquares
    }

    public class IterationsEarlyStopValidationInfo : ITrainingInfo
    {
        public int Tolerate { get; set; }

        public ErrorCaclulationType ErrorCalculation { get; set; }

        public void PerformTraining(ILearningMethod method, object model, IEnumerable validationData)
        {
            IRegressionAlgorithm regression = model as IRegressionAlgorithm;
            if (regression != null)
            {
                IErrorCalculation errorCalc;
                switch (ErrorCalculation)
                {
                    case ErrorCaclulationType.MeanSquare:
                        errorCalc = new ErrorCalculationMSE();
                        break;
                    case ErrorCaclulationType.RootMeanSquare:
                        errorCalc = new ErrorCalculationRMS();
                        break;
                    case ErrorCaclulationType.SumOfSquares:
                        errorCalc = new ErrorCalculationSSE();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                new SimpleLearn().PerformIterationsEarlyStop(method, regression, (IList<BasicData>) validationData,
                    Tolerate, errorCalc);
            }
            else
            {
                IClassificationAlgorithm classification = (IClassificationAlgorithm) model;
                new SimpleLearn().PerformIterationsClassifyEarlyStop(method, classification,
                    (IList<BasicData>) validationData, Tolerate);
            }
        }
    }


    public class NetworkTrainInfo
    {
        public NetworkTrainInfo()
        {
            Network = new AnonymousGeneralVariableInfo<object>()
            {
                VariableMode = VariableMode.Expression,
                FleeExpression = new FleeExpressionInfo<object>()
                {
                    Expression = "BasicNetwork.GetNetwork()",
                    Variables = new Variables(new VariableInfo[]
                    {
                        new GeneralVariableInfo()
                        {
                            Name = "BasicNetwork",
                            DotNetType = new DotNetType<INeuralNetwork>(typeof (BasicNetworkInfo)),
                            VariableMode = VariableMode.Instance
                        }
                    })
                }
            };
            LearningMethod = new AnonymousGeneralVariableInfo<ILearningMethodInfo>()
            {
                SubType = new EnabledFeature<SubDotNetType<ILearningMethodInfo>>(
                    new SubDotNetType<ILearningMethodInfo>(typeof (BackPropagationInfo))
                    ),
                VariableMode = VariableMode.Instance,
                Instance = new BackPropagationInfo()
            };
            TrainMode = new AnonymousGeneralVariableInfo<ITrainingInfo>()
            {
                VariableMode = VariableMode.Instance,
                Instance = new PerformIterationsInfo()
            };

            Data = new AnonymousGeneralVariableInfo<IEnumerable>();
            ValidationData = new AnonymousGeneralVariableInfo<IEnumerable>();
        }

        public AnonymousGeneralVariableInfo<object> Network { get; set; }

        public AnonymousGeneralVariableInfo<ILearningMethodInfo> LearningMethod { get; set; }

        public AnonymousGeneralVariableInfo<ITrainingInfo> TrainMode { get; set; }


        public AnonymousGeneralVariableInfo<IEnumerable> Data { get; set; }

        public AnonymousGeneralVariableInfo<IEnumerable> ValidationData { get; set; }


        public object Train(object owner, IContextLookup globalVars)
        {
            object theNetwork = Network.EvaluateTyped(owner, globalVars);
            IEnumerable theData = Data.EvaluateTyped(owner, globalVars);
            ILearningMethodInfo theLearningMethodInfo = LearningMethod.EvaluateTyped(owner, globalVars);

            ILearningMethod learningMethod = theLearningMethodInfo.GetLearningMethod(owner, globalVars, theNetwork,
                theData);
            IEnumerable theValidationData = ValidationData.EvaluateTyped(owner, globalVars);

            ITrainingInfo theTrainMode = TrainMode.EvaluateTyped(owner, globalVars);
            theTrainMode.PerformTraining(learningMethod, theNetwork, theValidationData);
            return theNetwork;
        }
    }

    public enum NetworkQueryMode
    {

    }

    public class NetworkQueryInfo
    {
        public AnonymousGeneralVariableInfo<object> Network { get; set; }

        public AnonymousGeneralVariableInfo<IEnumerable> Data { get; set; }


        public void Query(object owner, IContextLookup globalVars)
        {
            object theNetwork = Network.EvaluateTyped(owner, globalVars);
            IEnumerable theData = Data.EvaluateTyped(owner, globalVars);
        }
    }


    public class CntkEvaluator
    {

        public CntkEvaluator()
        {
            ModelPath = new FilePathInfo();
            Configuration = new CData();
            OutputKey = "ol.z";
            OutputSize = 10;
        }

        public FilePathInfo ModelPath { get; set; }

        public CData Configuration { get; set; }

        public string OutputKey { get; set; }

        public int OutputSize { get; set; }


        public List<float> Evaluate(object owner, IContextLookup globalVars, Dictionary<string, List<float>> inputs)
        {
            string theModelPath = ModelPath.GetMapPath(owner, globalVars);
            string configContent = Configuration.Value;
            return Evaluate(theModelPath, configContent, inputs, OutputKey, OutputSize);
        }

        public static  List<float> Evaluate(string modelFilePath, string configContent, Dictionary<string, List<float>> inputs, string outputKey, int outputSize)
        {
            Dictionary<string, List<float>> outputs;

            using (var model = new IEvaluateModelManagedF())
            {
                // Initialize model evaluator
                
                model.Init(configContent);

                // Load model
              
                model.LoadModel(modelFilePath);


                // We can call the evaluate method and get back the results (single layer)...
                // List<float> outputList = model.Evaluate(inputs, "ol.z", 10);

                return model.Evaluate(inputs, outputKey, outputSize);

                // ... or we can preallocate the structure and pass it in (multiple output layers)
                //var toReturn = new List<float>(outputSize);
                //for (int i = 0; i < outputSize; i++)
                //{
                //    toReturn.Add(1);
                //}
                //outputs = new Dictionary<string, List<float>>();

                //outputs.Add("ol.z", toReturn);
                //model.Evaluate(inputs, outputs);

                //Console.WriteLine("--- Output results ---");
                //foreach (var item in outputs)
                //{
                //    Console.WriteLine("Output layer: {0}", item.Key);
                //    foreach (var entry in item.Value)
                //    {
                //        Console.WriteLine(entry);
                //    }
                //}

            }
        }


    }


}