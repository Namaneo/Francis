//     This code was generated by a Reinforced.Typings tool. 
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { Progression } from './Progression';
import { WatchedItem } from './WatchedItem';

export class BotUser
{
	public id?: number;
	public userName?: string;
	public plexId?: string;
	public plexToken?: string;
	public ombiId?: string;
	public telegramId?: number;
	public progressions?: Progression[];
	public watchedItems?: WatchedItem[];
}