//     This code was generated by a Reinforced.Typings tool. 
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { Progression } from './Progression';
import { RequestType } from './RequestType';
import { RequestStatus } from './RequestStatus';

export class RequestProgression extends Progression
{
	public search?: string;
	public type?: RequestType;
	public status?: RequestStatus;
	public excludedIds?: number[];
}
