/* SPDX-License-Identifier: LGPL-2.1-only */
/*
 * $Id: TPCANMsgFD.java 18035 2023-10-26 13:21:42Z Fabrice $
 * @LastChange $Date: 2023-10-26 15:21:42 +0200 (Thu, 26 Oct 2023) $
 * 
 * PCANBasic JAVA Interface.
 *
 * Copyright (C) 2001-2023  PEAK System-Technik GmbH <www.peak-system.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 *
 * PCAN is a registered Trademark of PEAK-System Germany GmbH
 *
 * Author: 		 Jonathan Urban/Uwe Wilhelm/Fabrice Vergnaud
 * Contact:      <linux@peak-system.com>
 * Maintainer:   Fabrice Vergnaud <f.vergnaud@peak-system.com>
 */
package peak.can.basic;

import java.util.EnumSet;

/**
 * Defines a CAN FD message.
 */
public class TPCANMsgFD implements Cloneable
{    
    private int _id = 0;
    private EnumSet<TPCANMessageType> _type = EnumSet.noneOf(TPCANMessageType.class);
    private byte _dlc = 0;
    private byte _len = 0;
    private byte _data[] = new byte[64];

    /**
     * Default constructor
     */
    public TPCANMsgFD()
    {
    }

    /**
     * Constructs a new message object.
     * @param id the message id
     * @param type the message type
     * @param dlc the message data length code
     * @param data the message data
     */
    public TPCANMsgFD(int id, EnumSet<TPCANMessageType> type, byte dlc, byte[] data)
    {
        _id = id;
        _type = type;
        _dlc = dlc;
        _len = getLengthFromDLC(dlc);
        System.arraycopy(data, 0, _data, 0, _len);
    }

    /**
     * Sets the id of this message.
     * @param id the message id
     */
    public void setID(int id)
    {
        _id = id;
    }

    /**
     * Sets the data and length of this message.
     * @param data the message data
     * @param dlc the message length
     */
    public void setData(byte[] data, byte dlc)
    {
        setDlc(dlc);
        System.arraycopy(data, 0, _data, 0, _len);
    }

    /**
     * Sets the data length code of this message.
     * @param dlc the data length code of the message
     */
    public void setDlc(byte dlc)
    {
        _dlc = dlc;
        _len = getLengthFromDLC(dlc);
    }

    /**
     * Sets the type of this message.
     * @param type the message type
     */
    public void setType(EnumSet<TPCANMessageType> type)
    {
        _type = type;
    }
    
    /**
     * Sets the type of this message.
     * @param type the message type
     */
    public void setType(byte type)
    {
        EnumSet<TPCANMessageType> eset;
        
        eset = EnumSet.noneOf(TPCANMessageType.class);
        for (TPCANMessageType t : TPCANMessageType.values()) {
            if ((type & t.getValue()) == t.getValue())
                eset.add(t);                
        }
        // fix type PCAN_MESSAGE_STANDARD (value is 0)
        if (eset.contains(TPCANMessageType.PCAN_MESSAGE_EXTENDED) ||
                eset.contains(TPCANMessageType.PCAN_MESSAGE_STATUS) ||
                eset.contains(TPCANMessageType.PCAN_MESSAGE_ERRFRAME))
            eset.remove(TPCANMessageType.PCAN_MESSAGE_STANDARD);
        _type = eset;
    }

    /**
     * Gets the id of this message.
     * @return the message id
     */
    public int getID()
    {
        return _id;
    }

    /**
     * Gets the data of this message.
     * @return the message data
     */
    public byte[] getData()
    {
        return _data;
    }

    /**
     * Gets the data length code of this message.
     * @return the message length
     */
    public byte getDlc()
    {
        return _dlc;
    }
    
    /**
     * Gets the length of this message based on its DLC.
     * @return the message length
     */
    public byte getLengthFromDLC()
    {
        return getLengthFromDLC(_dlc);
    }
    /**
     * Gets the length of a message based on a DLC.
     * @param dlc data length code
     * @return the message length
     */
    public static byte getLengthFromDLC(byte dlc)
    {
        switch(dlc)
        {
            case 9:
                return 12;
            case 10:
                return 16;
            case 11:
                return 20;
            case 12:
                return 24;
            case 13:
                return 32;
            case 14:
                return 48;
            case 15:
                return 64;
            default:
                return dlc;
        }
    }

    /**
     * Gets the type of this message.
     * @return the message type
     */
    public EnumSet<TPCANMessageType> getTypeEnum()
    {
        return _type;
    }
    /**
     * Gets the type of this message.
     * @return the message type
     */
    public byte getType()
    {
        byte res;
        
        res = 0;
        for (TPCANMessageType t : _type)
            res |= t.getValue();
        return res;
    }    

    /**
     * Clones this message object.
     * @return The cloned message object.
     */
    @Override
    public Object clone()
    {
        TPCANMsgFD msg = null;
        try
        {
            msg = (TPCANMsgFD) super.clone();
            msg._data = _data.clone();
        }
        catch (CloneNotSupportedException e)
        {
            System.out.println(e.getMessage());
        }
        return msg;
    }
    
}
