// SimpleSample.cpp : Defines the entry point for the console application.
//
// PCAN-Basic-C-Console SimpleSample.cpp : Defines the entry point for the console application.
//
#include <stdio.h>
#include <conio.h>
#include <string.h>
#include <math.h>
#include <windows.h>
#include <time.h>
#include "pcanbasic.h"

// Function declaration
int LoadDLL();
int UnloadDLL();
bool GetFunctionAdress(HINSTANCE h_module);
//  switch listen Onyl Mode on/off
TPCANStatus SetListenOnlyMode(TPCANHandle g_hChannel, bool mode);


//typdef of Functions
typedef TPCANStatus (__stdcall *PCAN_Initialize)(TPCANHandle Channel, TPCANBaudrate Btr0Btr1, TPCANType HwType , DWORD IOPort , WORD Interrupt);
typedef TPCANStatus (__stdcall *PCAN_Uninitialize)( TPCANHandle Channel);
typedef TPCANStatus (__stdcall *PCAN_Reset)(TPCANHandle Channel);
typedef TPCANStatus (__stdcall *PCAN_GetStatus)(TPCANHandle Channel);
typedef TPCANStatus (__stdcall *PCAN_Read)(TPCANHandle Channel, TPCANMsg* MessageBuffer, TPCANTimestamp* TimestampBuffer);
typedef TPCANStatus (__stdcall *PCAN_Write)(TPCANHandle Channel, TPCANMsg* MessageBuffer);
typedef TPCANStatus (__stdcall *PCAN_FilterMessages)(TPCANHandle Channel, DWORD FromID, DWORD ToID, TPCANMode Mode);
typedef TPCANStatus (__stdcall *PCAN_GetValue)(TPCANHandle Channel, TPCANParameter Parameter, void* Buffer, DWORD BufferLength);
typedef TPCANStatus (__stdcall *PCAN_SetValue)(TPCANHandle Channel, TPCANParameter Parameter, void* Buffer, DWORD BufferLength);
typedef TPCANStatus (__stdcall *PCAN_GetErrorText)(TPCANStatus Error, WORD Language, LPSTR Buffer);

//declaration
PCAN_Initialize g_CAN_Initialize;
PCAN_Uninitialize g_CAN_Uninitialize;
PCAN_Reset g_CAN_Reset;
PCAN_GetStatus  g_CAN_GetStatus;
PCAN_Read g_CAN_Read;
PCAN_Write  g_CAN_Write;
PCAN_FilterMessages  g_CAN_FilterMessages;
PCAN_GetValue  g_CAN_GetValue;
PCAN_SetValue  g_CAN_SetValue;
PCAN_GetErrorText  g_CAN_GetErrorText;

// name of DLL
char g_LibFileName[] = "PCANBasic";
LPCWSTR g_LibFileP = (LPCWSTR)g_LibFileName;
//DLL Instance Handle
HINSTANCE g_i_DLL;
// TPCANHandle
TPCANHandle g_hChannel;
TPCANBaudrate g_Baudrate;
// nur für non PNP
TPCANType g_CANType;
DWORD g_IOPort;
WORD g_Int;

int main(int argc, char *argv[])
{
	int ret,i;
	TPCANStatus CANStatus;
	TPCANMsg SendMessageBuffer; 
	TPCANMsg ReadMessageBuffer; 
	TPCANTimestamp MessageTimeStamp;
    
	printf("PCAN-Basic SimpleSample - Demo (c)2013 PEAK-System Technik GmbH\n");
	printf("using 500k\n");
	
	//Default USB Channel1
	g_hChannel = PCAN_USBBUS1;


	if(argc>1) //we have a parameter...
	{
		if(strcmp(argv[1], "help")==0)
		{
			printf("usage: SimpleSample ###\n where ### is usb | pci | pcc\n");
			printf("press any key to close");
			_getch();
			exit(0);
		}
		if(strcmp(argv[1], "usb")==0)
		{
			g_hChannel = PCAN_USBBUS1;
			printf("use PCAN-USB Channel 1\n");
		}
		if(strcmp(argv[1], "pci")==0)
		{
			g_hChannel = PCAN_PCIBUS1;
			printf("use PCAN-PCI Channel 1\n");
		}
		if(strcmp(argv[1], "pcc")==0)
		{
			g_hChannel = PCAN_PCCBUS1;
			printf("use PCAN-PC Cards Channel 1\n");
		}
	}
	ret = LoadDLL();
	if(ret!=0)
	{
	 printf("Load DLL: %i", ret);
	 exit(-1);
	}


	// Init der PCANBasic Applikation
	CANStatus = g_CAN_Initialize(g_hChannel, PCAN_BAUD_500K , 0, 0, 0); 
	if(CANStatus!=PCAN_ERROR_OK)
	{
		printf("Error while Init CAN Interface: 0x%x \n",CANStatus);
		//DLL entladen..
		UnloadDLL();
		printf("press any key to close");
		_getch();
		//und raus
		exit(-1);

	}
	printf("press any key to start\n");
	_getch();

	// Customer implementation
	unsigned long ulLoopTime=0;
	
	#define LOOP_TIME  5 // Set Timeout in Seconds...for test i use 5Seconds...

    // Get messages until all have come in.
    // But time out for possible errors; do not hang on a faulty board!
    ulLoopTime = (unsigned long) time (NULL) + LOOP_TIME;

    do
      {
         do
         {
            Sleep (5);   // 5mSec
            CANStatus = g_CAN_Read(PCAN_USBBUS1, &ReadMessageBuffer, &MessageTimeStamp);
		 }while(CANStatus == PCAN_ERROR_QRCVEMPTY && ((unsigned long)time(NULL) < ulLoopTime)); // I read in a loop until the driver told me that the queue is empty, or timer passed
		 //  Now i have the info that the result was not PCAN_ERROR_QRCVEMPTY - i need to check more!

		 if(ReadMessageBuffer.MSGTYPE == PCAN_MESSAGE_STATUS)
		 {
			printf("We received a Status Message - please check the Databytes for more information.\n");
			//Reset the time off
			ulLoopTime = (unsigned long) time (NULL) + LOOP_TIME;
		 }
		 else
		 {
			if(CANStatus == PCAN_ERROR_OK) //now we have e real CAN Frame..
			{	
				// We only need CAN Frames with STD ID and with ending ID F8 (0x12F8, 0x01F8 etc.)
			  if( (ReadMessageBuffer.MSGTYPE == PCAN_MESSAGE_STANDARD) && ( ((ReadMessageBuffer.ID&0xFF)^0xF8)==0) )  
			  {
				printf("Receice a CAN-Frame ID: %d\n", ReadMessageBuffer.ID);
			    //Reset the time off
				ulLoopTime = (unsigned long) time (NULL) + LOOP_TIME;
		        // We have a message.  Save and report the data.
				if(ReadMessageBuffer.DATA[0]= 0x2)   // 'address' lo byte --> circuit no.
				{
		         if ((ReadMessageBuffer.DATA[1] == 0x3) && (ReadMessageBuffer.LEN == 8))
				 {
					 printf("Received ID: %u with DLC: %d - ", ReadMessageBuffer.ID, ReadMessageBuffer.LEN);
					 printf("Value1: %d\n",(ReadMessageBuffer.DATA[2]*256)+ReadMessageBuffer.DATA[3]);
				 }//end if DATA1 ==0x03...
				 else
					if ((ReadMessageBuffer.DATA[1] == 0x02) && (ReadMessageBuffer.LEN == 8))
					{
						printf("Received ID: %u with DLC: %d - ", ReadMessageBuffer.ID, ReadMessageBuffer.LEN);
						printf("Value2: %d\n",(ReadMessageBuffer.DATA[2]*256)+ReadMessageBuffer.DATA[3]);
					}//end if DATA1 ==0x02...
				}
				//Reset the time off
				ulLoopTime = (unsigned long) time (NULL) + LOOP_TIME;
			  }
			  else 
				if (ReadMessageBuffer.LEN > 0)  // A DLC could also be 0..but we only show ID´s with DATA
				{
					// report other messages
					printf("Receive - ");
					printf("ID: 0x%x\tDLC: %d\tMSGTYPE: %d\t",ReadMessageBuffer.ID, ReadMessageBuffer.LEN, ReadMessageBuffer.MSGTYPE);
					for (i = 0; i < ReadMessageBuffer.LEN; i++)
						printf (" %02X", ReadMessageBuffer.DATA[i]);
						printf ("\r\n");
				    //Reset the time off
					ulLoopTime = (unsigned long) time (NULL) + LOOP_TIME;
				}
			} //endif STATUS OK
		}//else Status Message
      }while (( (unsigned long) time (NULL) < ulLoopTime) );
	printf("end of loop ... wait for key...\n");
	do{}
	while(!_kbhit()); //Solange bis Tatse gedrückt wird...

	//DLL entladen..
	UnloadDLL();
	return 0;
}



/***************************************************************************************

 Dynamic Load of the DLL and all function pointer

****************************************************************************************/

//
// Function: Load DLL
// Parameter: none
// ret value: 0 if OK, -1 if DLL not found or can not open, -2 if function pointer not found
//
// load the DLL and get function pointers
//


int LoadDLL()
{
	if(g_i_DLL==NULL)
	{
		g_i_DLL = LoadLibrary(g_LibFileName);
		if(g_i_DLL == NULL)
		{
			printf("ERROR: can not load pcanbasic.dll\n");
			return -1;
		}	
		else
		{
			printf("DLL Handle: 0x%x\n",g_i_DLL);
			if(GetFunctionAdress( g_i_DLL )==true)
			{
				printf("Load function adress for pcan_basic.dll\n");
			}
			else
			{
				printf("ERROR: can not load Function Adress\n");
				return -2;
			}
		}
	}
	return 0;
}


//
// Function: GetFunctionAdress
// Parameter: instance of DLL
// ret value: true if OK false if pointer not vallid
//
// load the function pointer from the DLL spec. by handle
//



bool GetFunctionAdress(HINSTANCE h_module)
{
  //Lade alle Funktionen
  if(h_module == NULL)
   return false;

  g_CAN_Initialize = (PCAN_Initialize) GetProcAddress(h_module, "CAN_Initialize");
  if(g_CAN_Initialize == NULL)
   return false;

  g_CAN_Uninitialize = (PCAN_Uninitialize) GetProcAddress(h_module, "CAN_Uninitialize");
  if(g_CAN_Uninitialize == NULL)
   return false;

  g_CAN_Reset = (PCAN_Reset) GetProcAddress(h_module, "CAN_Reset");
  if(g_CAN_Reset == NULL)
   return false;

  g_CAN_GetStatus = (PCAN_GetStatus) GetProcAddress(h_module, "CAN_GetStatus");
  if(g_CAN_GetStatus == NULL)
   return false;

  g_CAN_Read = (PCAN_Read) GetProcAddress(h_module, "CAN_Read");
  if(g_CAN_Read == NULL)
   return false;

  g_CAN_Write = (PCAN_Write) GetProcAddress(h_module, "CAN_Write");
  if(g_CAN_Write == NULL)
   return false;

  g_CAN_FilterMessages = (PCAN_FilterMessages) GetProcAddress(h_module, "CAN_FilterMessages");
  if(g_CAN_FilterMessages == NULL)
   return false;

  g_CAN_GetValue = (PCAN_GetValue) GetProcAddress(h_module, "CAN_GetValue");
  if(g_CAN_GetValue == NULL)
   return false;

  g_CAN_SetValue = (PCAN_SetValue) GetProcAddress(h_module, "CAN_SetValue");
  if(g_CAN_SetValue == NULL)
   return false;

  g_CAN_GetErrorText = (PCAN_GetErrorText) GetProcAddress(h_module, "CAN_GetErrorText");
  if(g_CAN_GetErrorText == NULL)
   return false;

  return true;
}
//
// Function: Unload DLL
// Parameter: none
// ret value: 0 if OK 
//
// unload the DLL and free all pointers
//

int UnloadDLL()
{
 if(g_i_DLL)
 {
  FreeLibrary(g_i_DLL);
  g_CAN_Initialize = NULL;
  g_CAN_Uninitialize = NULL;
  g_CAN_Reset = NULL;
  g_CAN_GetStatus = NULL;
  g_CAN_Read = NULL;
  g_CAN_Write = NULL;
  g_CAN_FilterMessages = NULL;
  g_CAN_GetValue = NULL;
  g_CAN_SetValue = NULL;
  g_CAN_GetErrorText = NULL;
  return 0;
 }
 return -1;
}