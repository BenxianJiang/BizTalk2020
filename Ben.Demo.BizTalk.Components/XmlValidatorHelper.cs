// Copyright 2019, DXC
//
// Developed by DXC Technology Adelaide 2019.
// unpublished work created in 2019. This work is a trade secret
// and unauthorised use or copying is prohibited.
// _______________________________________________________________________
// Project Name		: AIBP - Auto Indexing
// File Name        : XmlValidatorHelper.cs
// Description      : Class to contain XML Validation Helper Methods
// _______________________________________________________________________
// Date			ChangeID	Developer		        Description
// ==========	==========	=============	        =====================
// 2019-02-19	            Mandar Dharmadhikari 	Initial Release                                             
// _______________________________________________________________________

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using Microsoft.BizTalk.Streaming;
using log4net;

namespace Ben.Demo.BizTalk.Components
{
    /// <summary>
    /// Helper Class to Perform Validation of the XmlRequest against matching Schema and calloect the errors.
    /// </summary>
    public class XmlValidatorHelper
    {
        private int _errorsCount = 0;
        private int _maxErrorsCount;

        private ILog _logger;

        public ILog Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        //name of the file against which the validation is performed
        private string _fileName;


        //private Common.ProcessStatus _processStatus;



        //public Common.ProcessStatus ProcessStatus
        //{
        //    get { return _processStatus; }
        //}

        
        //public XmlValidatorHelper(int maxErrorCount, string fileName)
        //{
            
        //    _processStatus = new Common.ProcessStatus();
        //    _processStatus.Status = Common.StatusType.Success;
        //    _fileName = fileName;
            
        //}

        

        readonly StringBuilder sb = new StringBuilder(string.Empty);

        /// <summary>
        /// Handles the Validation Event and Appends the Errors to the String Builder
        /// </summary>
        /// <param name="sender">Contains Reference to the object that raised the exception</param>
        /// <param name="args">Contains Event Data</param>
        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            //ValidationErrorCollectionValidationError error = new ValidationErrorCollectionValidationError();
            var errorText = string.Format("Line: {0}, Position: {1}, Error: {2}\r\n", args.Exception.LineNumber, args.Exception.LinePosition, args.Message);

            //Get the Error Type stored in LookUp Db and map it to the Common Error Type
            //ErrorType canonicalError = ErrorDbResourceLoader.GetError(Constants.ErrorCodes.SchemaValidationErrorCode);

            //Common.ErrorType error = new Common.ErrorType()
            //{
            //    Category = canonicalError.Category,
            //    Code = canonicalError.Code,
            //    Message = canonicalError.Message,
            //    Source = canonicalError.Source,
            //    TimeOccurred = DateTime.Now
            //};//new Common.ErrorType(Constants.ErrorCodes.SchemaValidationCategory, Constants.ErrorCodes.SchemaValidationErrorCode, "SchemaValidation", "{0}", Constants.ErrorCodes.BiztalkSource);
            //error.Message = string.Format(error.Message, errorText);
            
           // _processStatus.Errors = Utils.AddItemToArray<Common.ErrorType>(_processStatus.Errors, error);
            
            sb.Append(errorText);

            _errorsCount++;

        }

        /// <summary>
        /// Performs the validation of Xml against the deployed Schema.
        /// </summary>
        /// <param name="streamDocument">Stream representation of the incoming Xml Message</param>
        /// <param name="schemas">Set Of Schemas against which the Xml is validated</param>
        /// <param name="maxErrorCount">Max Number of Errors that should be reported</param>
        /// <param name="mesageType">Message Type of the Incoming Message</param>
        /// <param name="messageId">Message Id of the incoming message.</param>
        public void Validate(Stream streamDocument, XmlSchemaSet schemas, int maxErrorCount, string messageType, string messageId)
        {
            _maxErrorsCount = maxErrorCount;

            bool complete = false;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

            using(XmlReader reader = XmlReader.Create(new XmlTextReader(streamDocument), settings))
            {
                while (reader.Read())
                {
                    if (_errorsCount >= _maxErrorsCount)
                    {
                        reader.Close();
                        break;
                    }
                }

                complete = reader.ReadState == ReadState.EndOfFile;
            }

            if (_errorsCount > 0)
            {
               // _processStatus.Status = Common.StatusType.Error;
                //Throw Custom Exception Here
                string errorDescription = string.Format("Request Id {0}: XML validation failed for {1}. Following are the errors {2}", _fileName, messageType, sb.ToString());

                _logger.Error(errorDescription);
               
            }

         
        }

        /// <summary>
        /// Wrapper To perform XmlValidation
        /// </summary>
        /// <param name="docSpec">The Document NameSpec derieved for the message</param>
        /// <param name="maxErrorCount">maximum number of Schema validation errors to be captured</param>
        /// <param name="stream">Stream representation of the incoming message</param>
        /// <param name="messageType">Message Type of the incoming message</param>
        /// <param name="messageId">Id of the incoming message</param>
        public void ValidationWrapper(IDocumentSpec docSpec, int maxErrorCount, VirtualStream stream, string messageType, string messageId)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();

            try
            {
                foreach (var schema in docSpec.GetSchemaCollection())
                {
                    schemas.Add(schema);
                }
            }
            catch (XmlSchemaException ex)
            {
                string errorDescription = string.Format("An error occured while loading the schemas. Error Message: {0} \r\nLine: {1}, Position: {2} \r\n", ex.Message, ex.LineNumber, ex.LinePosition);

                throw new Exception(errorDescription);
            }
            catch (Exception sysEx)
            {
                throw new Exception("An error occured while loading the schemas. Error detail: " + sysEx.ToString());
            }

            Validate(stream, schemas, maxErrorCount, messageType, messageId);

            
        }

        /// <summary>
        /// Add the Error Message in case of Invalid Namespace in incoming message
        /// </summary>
        /// <param name="message">Exception message</param>
        public void AddInvalidMessageTypeError(string message)
        {
            //Common.ErrorType error = new Common.ErrorType(Constants.ErrorCodes.SchemaValidationCategory, "7000", "Validation Error", "{0}", Constants.ErrorCodes.BiztalkSource);
            //error.Message = string.Format(error.Message, message);
            //_processStatus.Status = Common.StatusType.Error;
            //_processStatus.Errors = Utils.AddItemToArray<Common.ErrorType>(_processStatus.Errors, error);

 
        }

        
    }
}
